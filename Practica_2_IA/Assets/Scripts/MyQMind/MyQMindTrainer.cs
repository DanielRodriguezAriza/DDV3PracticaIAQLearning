#region Copyright
// MIT License
// 
// Copyright (c) 2023 David María Arribas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using NavigationDJIA.Interfaces;
using NavigationDJIA.World;
using QMind.Interfaces;
using UnityEngine;

namespace QMind
{
    public class MyQMindTrainer : IQMindTrainer
    {
        #region PrivateVariables

        QMindTrainerParams qMindTrainerParams;
        private WorldInfo worldInfo;
        INavigationAlgorithm navigationAlgorithm;

        private int currentEpisode;
        private int currentStep;

        private QTable qTable;


        private const float penaltyScore = -1.0f;
        private const float rewardScore = 100.0f;
        private const float neutralScore = 0.0f;

        #endregion

        #region PublicVariables

        public int CurrentEpisode { get { return currentEpisode; } }
        public int CurrentStep { get { return currentStep; } }
        public CellInfo AgentPosition { get; private set; }
        public CellInfo OtherPosition { get; private set; }
        public float Return { get { return 0; } }
        public float ReturnAveraged { get { return 1; } }
        public event EventHandler OnEpisodeStarted;
        public event EventHandler OnEpisodeFinished;

        #endregion

        #region PublicMethods

        public void Initialize(QMindTrainerParams qMindTrainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            this.qMindTrainerParams = qMindTrainerParams;
            this.worldInfo = worldInfo;
            this.navigationAlgorithm = navigationAlgorithm;
            this.navigationAlgorithm.Initialize(this.worldInfo);

            this.qTable = new QTable();

            LoadQTable();

            Debug.Log("QMindTrainerDummy: initialized");

            StartEpisode(0);
        }

        public void DoStep(bool train)
        {
            //Debug.Log("QMindTrainerDummy: DoStep");

            //Debug.Log($"{qTable.GetFileName()}");

            //Debug.Log($"max steps : {qMindTrainerParams.maxSteps}");

            QTableState state = GetState();
            QTableAction action = GetAction(state);
            float reward = GetReward(state, action);

            UpdateQTable(state, action, reward);

            if (((qMindTrainerParams.maxSteps >= 0) && ((currentStep + 1) > qMindTrainerParams.maxSteps)) || reward < 0) // -1 means infinite max steps.
            {
                NextEpisode();
            }

            MovePlayer();
            MoveAgent(action);

            ++this.currentStep;
        }

        #endregion

        #region PrivateMethods

        private void StartEpisode(int episodeIdx)
        {
            currentEpisode = episodeIdx;
            AgentPosition = worldInfo.RandomCell();
            OtherPosition = worldInfo.RandomCell();
            OnEpisodeStarted?.Invoke(this, EventArgs.Empty);
            this.currentStep = 0;

            int val = currentEpisode % qMindTrainerParams.episodesBetweenSaves;
            Debug.Log($"val : {val}");

            SaveQTable();
        }

        private void NextEpisode()
        {
            StartEpisode(currentEpisode + 1);
        }

        private Directions GetRandomDirection()
        {
            Directions[] directions = new Directions[4] { Directions.Up, Directions.Right, Directions.Down, Directions.Left };
            int dir = UnityEngine.Random.Range(0, 4);
            return directions[dir];
        }

        private Directions GetActionDirection(QTableAction action)
        {
            Directions[] directions = new Directions[4] { Directions.Up, Directions.Right, Directions.Down, Directions.Left };
            int dir = (int)action;
            return directions[dir];
        }

        private QTableAction GetRandomAction()
        {
            QTableAction[] actions = new QTableAction[4] { QTableAction.GoNorth, QTableAction.GoEast, QTableAction.GoSouth, QTableAction.GoWest};
            int action = UnityEngine.Random.Range(0, 4);
            return actions[action];
        }

        private QTableAction GetBestAction(QTableState state)
        {
            float[] qValues = new float[4] {
                qTable.GetQ(state, QTableAction.GoNorth),
                qTable.GetQ(state, QTableAction.GoEast),
                qTable.GetQ(state, QTableAction.GoSouth),
                qTable.GetQ(state, QTableAction.GoWest)
            };
            float maxValue = qValues[0];
            float maxIndex = 0;
            for (int i = 0; i < qValues.Length; ++i)
            {
                if (qValues[i] > maxValue)
                {
                    maxValue = qValues[i];
                    maxIndex = i;
                }
            }
            return (QTableAction)maxIndex;
        }


        private void SaveQTable()
        {
            if (currentEpisode > 0 && currentEpisode % qMindTrainerParams.episodesBetweenSaves == 0)
            {
                qTable.SaveTable();
            }
        }

        private void LoadQTable()
        {
            qTable.LoadTable();
            qTable.DebugPrintTableInfo();
        }


        private QTableState GetState(CellInfo cell)
        {
            QTableState state = new QTableState(
                GetIsWalkable(cell, Directions.Up),
                GetIsWalkable(cell, Directions.Right),
                GetIsWalkable(cell, Directions.Down),
                GetIsWalkable(cell, Directions.Left),
                GetOtherIsInDirection(cell, Directions.Up),
                GetOtherIsInDirection(cell, Directions.Right),
                GetOtherIsInDirection(cell, Directions.Down),
                GetOtherIsInDirection(cell, Directions.Left),
                GetOtherDistance(cell)
            );
            return state;
        }

        private bool GetIsWalkable(CellInfo cell, Directions dir)
        {
            return worldInfo.NextCell(cell, dir).Walkable;
        }

        private bool GetOtherIsInDirection(CellInfo cell, Directions dir)
        {
            float otherX = OtherPosition.x;
            float otherY = OtherPosition.y;
            float selfX = cell.x;
            float selfY = cell.y;
            switch (dir)
            {
                case Directions.Up:
                    return selfY < otherY;
                case Directions.Right:
                    return selfX < otherX;
                case Directions.Down:
                    return selfY > otherY;
                case Directions.Left:
                    return selfX > otherX;
                default:
                    return false;
            }
        }

        private QTableDistances GetOtherDistance(CellInfo cell)
        {
            float distance = cell.Distance(OtherPosition, CellInfo.DistanceType.Manhattan);
            if (distance >= 10) return QTableDistances.Far;
            if (distance >= 5) return QTableDistances.Middle;
            return QTableDistances.Close;
        }

        private QTableState GetState()
        {
            return GetState(AgentPosition);
        }

        private QTableState GetNextState(QTableAction action)
        {
            switch (action)
            {
                case QTableAction.GoNorth:
                    return GetState(worldInfo.NextCell(AgentPosition, Directions.Up));
                case QTableAction.GoEast:
                    return GetState(worldInfo.NextCell(AgentPosition, Directions.Right));
                case QTableAction.GoSouth:
                    return GetState(worldInfo.NextCell(AgentPosition, Directions.Down));
                case QTableAction.GoWest:
                    return GetState(worldInfo.NextCell(AgentPosition, Directions.Left));
                default:
                    return GetState(AgentPosition);
            }
        }

        private QTableAction GetAction(QTableState state)
        {
            float n = UnityEngine.Random.Range(0.0f, 1.0f);

            if (n <= qMindTrainerParams.epsilon)
                return GetRandomAction();

            return GetBestAction(state);
        }

        private float GetReward(QTableState state, QTableAction action)
        {
            bool cannotWalk =
                (!state.NorthIsWalkable && action == QTableAction.GoNorth) ||
                (!state.EastIsWalkable && action == QTableAction.GoEast) ||
                (!state.SouthIsWalkable && action == QTableAction.GoSouth) ||
                (!state.WestIsWalkable && action == QTableAction.GoWest)
                ;

            bool caught = OtherPosition.Distance(AgentPosition, CellInfo.DistanceType.Manhattan) == 0;

            if (cannotWalk || caught)
                return penaltyScore;

            QTableState nextState = GetNextState(action);
            if (nextState.EnemyDistance == QTableDistances.Far)
                return rewardScore;

            return neutralScore;
        }

        private void UpdateQTable(QTableState state, QTableAction action, float reward)
        {
            qTable.UpdateQ(state, GetNextState(action), action, reward, qMindTrainerParams.alpha, qMindTrainerParams.gamma);
        }

        private void MovePlayer()
        {
            var path = navigationAlgorithm.GetPath(OtherPosition, AgentPosition, 512);
            if (path != null && path.Length > 0) OtherPosition = path[0];
        }

        private void MoveAgent(QTableAction action)
        {
            AgentPosition = worldInfo.NextCell(AgentPosition, GetActionDirection(action));
        }

        #endregion

    }
}