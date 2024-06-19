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

        public void Initialize(QMindTrainerParams qMindTrainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            this.qMindTrainerParams = qMindTrainerParams;
            this.worldInfo = worldInfo;
            this.navigationAlgorithm = navigationAlgorithm;
            this.navigationAlgorithm.Initialize(this.worldInfo);

            this.qTable = new QTable();

            qTable.SaveTable();

            Debug.Log("QMindTrainerDummy: initialized");

            StartEpisode(0);
        }

        public void DoStep(bool train)
        {
            //Debug.Log("QMindTrainerDummy: DoStep");

            Debug.Log($"{qTable.GetFileName()}");

            var path = navigationAlgorithm.GetPath(OtherPosition, AgentPosition, 512);
            if(path != null && path.Length > 0) OtherPosition = path[0];

            QTableState state = new QTableState(
                worldInfo.NextCell(AgentPosition, Directions.Up).Walkable,
                worldInfo.NextCell(AgentPosition, Directions.Right).Walkable,
                worldInfo.NextCell(AgentPosition, Directions.Down).Walkable,
                worldInfo.NextCell(AgentPosition, Directions.Left).Walkable,
                false,
                false,
                false,
                false,
                0
            );

            AgentPosition = worldInfo.NextCell(AgentPosition, GetRandomDirection());

            ++this.currentStep;
        }

        private void StartEpisode(int episodeIdx)
        {
            currentEpisode = episodeIdx;
            AgentPosition = worldInfo.RandomCell();
            OtherPosition = worldInfo.RandomCell();
            OnEpisodeStarted?.Invoke(this, EventArgs.Empty);
        }

        private Directions GetRandomDirection()
        {
            Directions[] directions = new Directions[4] { Directions.Up, Directions.Right, Directions.Down, Directions.Left };
            int dir = UnityEngine.Random.Range(0, 4);
            return directions[dir];
        }

    }
}