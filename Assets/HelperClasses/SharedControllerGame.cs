using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace GameFacilities
{
	internal class SharedControllerGame
	{
		private static SharedControllerGame instance;
		private int lastScore = -1;
		private bool alreadyPlayed = false;
		private List<List<Vector2>> tasks;
		public static SharedControllerGame Shared
		{
			get
			{
				if (SharedControllerGame.instance == null)
				{
					SharedControllerGame.instance = new SharedControllerGame();
				}
				return SharedControllerGame.instance;
			}
		}
		public int LastScore
		{
			get
			{
				return this.lastScore;
			}
			set
			{
				this.lastScore = value;
				this.alreadyPlayed = true;
			}
		}
		public bool AlreadyPlayed
		{
			get
			{
				return this.alreadyPlayed;
			}
		}
		public int TasksCount
		{
			get
			{
				return this.tasks.Count;
			}
		}
		public SharedControllerGame()
		{
			this.initLevels();
		}
		private int stringToInt(string s)
		{
			int result;
			if (!int.TryParse(s, out result))
			{
				throw new ApplicationException("Wrong task data");
			}
			return result;
		}
		private Vector2 stringToVector2(string s)
		{
			string[] array = s.Split(new char[]
			{
				' '
			});
			if (array.Length != 2)
			{
				throw new ApplicationException("Wrong task data");
			}
			int num;
			int num2;
			if (!int.TryParse(array[0], out num) || !int.TryParse(array[1], out num2))
			{
				throw new ApplicationException("Wrong task data");
			}
			return new Vector2((float)num, (float)num2);
		}

        private void writeDefaultTasks(string path)
        {
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (8, 8)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (5, 5), new Vector2(10, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (0, 10), new Vector2(10, 10), new Vector2(10, 0), new Vector2(0, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (5, 5), new Vector2(10, 0), new Vector2(0, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (5, 5), new Vector2(10, 0), new Vector2(15, 5), new Vector2(20, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (5, 5), new Vector2(10, 0), new Vector2(15, 5), new Vector2(20, 0), new Vector2(0, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), new Vector2 (15, 10), new Vector2(15, 0), new Vector2(0, 10), new Vector2(0, 0)});
            tasks.Add(new List<Vector2>() {new Vector2 (0, 0), 
                                           new Vector2 (4, 5), 
                                           new Vector2(0, 10), 
                                           new Vector2(11, 10), 
                                           new Vector2(7, 5),
                                           new Vector2(11, 0),
                                           new Vector2(0, 0)});

            tasks.Add(new List<Vector2>() {new Vector2 (5, 5), 
                                           new Vector2 (0, 0), 
                                           new Vector2(-5, 5), 
                                           new Vector2(0, 8), 
                                           new Vector2(5, 5),
                                           new Vector2(8, 5),
                                           new Vector2(13, 8),
                                           new Vector2(16, 5),
                                           new Vector2(13, 0),
                                           new Vector2(8, 5)});

            for (int i = 0; i < tasks.Count; ++i)
                using (StreamWriter writer = new StreamWriter(path + @"/" + (i + 1).ToString()))
                {
                    foreach (Vector2 p in tasks[i])
                        writer.WriteLine(p.x + " " + p.y);

                    writer.Close();
                }
        }

        private List<Vector2> readTask(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            List<Vector2> res = new List<Vector2>();
            string s = String.Empty;

            while (!streamReader.EndOfStream)
            {
                s = streamReader.ReadLine();
                res.Add(this.stringToVector2(s));
            }
            streamReader.Close();

            return res;
        }

		private void initLevels()
		{
            this.tasks = new List<List<Vector2>>();
            string defPath = "Gestures_Data/Tasks";
            if (!Directory.Exists(defPath))
            {
                Directory.CreateDirectory(defPath);
                writeDefaultTasks(defPath);
            }
            else
            {
                var files = Directory.GetFiles(defPath).OrderBy(f => new FileInfo(f).CreationTime);

                foreach (var file in files)
                    tasks.Add(readTask(file));       
            }
		}
		public List<Vector2> GetTask(int id)
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.tasks[id].Count; i++)
			{
				list.Add(this.tasks[id][i]);
			}
			return list;
		}
	}
}
