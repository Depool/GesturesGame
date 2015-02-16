using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameFacilities
{
    class GameSessionInfo
    {
        public struct TaskInfo
        {
            private List<Vector2> task;
            private float timeAllowed;

            public List<Vector2> Task
            {
                get { return task; }
            }

            public float TimeAllowed
            {
                get { return timeAllowed; }
            }

            public TaskInfo(List<Vector2> task, float timeAllowed)
            {
                this.task = task;
                this.timeAllowed = timeAllowed;
            }
        }

        private int score;
        private int curTask;
        private float timeForTask = 30.0f;

        public int Score
        {
            get
            {
                return score;
            }
        }

        public GameSessionInfo()
        {
            score = 0;
            curTask = 0;
        }


        public TaskInfo NextTask()
        {
            if (curTask == SharedControllerGame.Shared.TasksCount)
            {
                curTask = 0;
                timeForTask = (float)Math.Truncate(timeForTask * 0.75);
            }

            List<Vector2> resTask = SharedControllerGame.Shared.GetTask(curTask);
            curTask++;
            return new TaskInfo(resTask, timeForTask);
        }

        public void AddScorePoint()
        {
            score++;
        }
    }
}
