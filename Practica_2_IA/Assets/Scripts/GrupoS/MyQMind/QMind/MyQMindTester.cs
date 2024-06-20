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

using NavigationDJIA.World;
using QMind.Interfaces;
using UnityEngine;

namespace QMind
{
    public class MyQMindTester : IQMind
    {
        private WorldInfo worldInfo;
        private QTable qTable;

        public void Initialize(WorldInfo worldInfo)
        {
            this.worldInfo = worldInfo;
            this.qTable = new QTable();

            qTable.LoadTable();

            Debug.Log("MyQMindTester: Initialized");
        }

        public CellInfo GetNextStep(CellInfo currentPosition, CellInfo otherPosition)
        {
            Debug.Log("MyQMindTester: GetNextStep");
            QTableState currentState = GetState(currentPosition, otherPosition);
            QTableAction bestAction = qTable.GetBestAction(currentState);
            CellInfo ans = MoveAgent(currentPosition, bestAction);
            return ans;
        }

        private QTableState GetState(CellInfo cell, CellInfo other)
        {
            QTableState state = new QTableState(
                GetIsWalkable(cell, Directions.Up),
                GetIsWalkable(cell, Directions.Right),
                GetIsWalkable(cell, Directions.Down),
                GetIsWalkable(cell, Directions.Left),
                GetOtherIsInDirection(cell, other, Directions.Up),
                GetOtherIsInDirection(cell, other, Directions.Right),
                GetOtherIsInDirection(cell, other, Directions.Down),
                GetOtherIsInDirection(cell, other, Directions.Left),
                GetOtherDistance(cell, other)
            );
            return state;
        }

        private bool GetIsWalkable(CellInfo cell, Directions dir)
        {
            return worldInfo.NextCell(cell, dir).Walkable;
        }

        private bool GetOtherIsInDirection(CellInfo cell, CellInfo other, Directions dir)
        {
            float otherX = other.x;
            float otherY = other.y;
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

        private QTableDistances GetOtherDistance(CellInfo cell, CellInfo other)
        {
            float distance = cell.Distance(other, CellInfo.DistanceType.Manhattan);
            if (distance >= 10) return QTableDistances.Far;
            if (distance >= 5) return QTableDistances.Middle;
            return QTableDistances.Close;
        }

        private CellInfo MoveAgent(CellInfo cell, QTableAction action)
        {
            Directions[] directions = new Directions[4] { Directions.Up, Directions.Right, Directions.Down, Directions.Left };
            CellInfo ans = worldInfo.NextCell(cell, directions[(int)action]);
            ans = ans.Walkable ? ans : null;
            return ans;
        }
    }
}