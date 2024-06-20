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

            MovePlayer();

            QTableState state = GetState();
            QTableAction action = GetAction();
            float reward = GetReward();

            MoveAgent(action);

            UpdateQTable(state, action, reward);

            ++this.currentStep;

            if (qMindTrainerParams.maxSteps >= 0 && currentStep > qMindTrainerParams.maxSteps) // -1 means infinite max steps.
            {
                NextEpisode();
            }
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
            //qTable.DebugPrintTableInfo();
        }


        private QTableState GetState(CellInfo cell)
        {
            QTableState state = new QTableState(
                worldInfo.NextCell(cell, Directions.Up).Walkable,
                worldInfo.NextCell(cell, Directions.Right).Walkable,
                worldInfo.NextCell(cell, Directions.Down).Walkable,
                worldInfo.NextCell(cell, Directions.Left).Walkable,
                false,
                false,
                false,
                false,
                0
            );
            return state;
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

        private QTableAction GetAction()
        {
            QTableAction action = QTableAction.GoEast;
            return action;
        }

        private float GetReward()
        {
            return 0.0f;
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
            float n = UnityEngine.Random.Range(0.0f, 1.0f);
            if(n <= qMindTrainerParams.epsilon)
                AgentPosition = worldInfo.NextCell(AgentPosition, GetRandomDirection());
            else
                AgentPosition = worldInfo.NextCell(AgentPosition, GetActionDirection(action));
        }

        #endregion

    }
}