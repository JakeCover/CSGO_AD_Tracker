﻿using System;
using System.Drawing;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CSGO_AD_Tracker_Forms_net5
{
    class FlowingGraph
    {
        //Point information
        int pointArraySize;
        int pointSpread;
        IndexedList<PointF> pointQueue = new IndexedList<PointF>();

        //Debug
        Random debugRandom = new Random();
        bool debug;

        //Picturebox Info
        PictureBox box;
        Point boxPosition;
        Size boxSize;
        Color boxColor;
        float graphX;
        float thickness;

        //Timers
        System.Timers.Timer update;

        public FlowingGraph(bool debug, Form form, Point position, Size size, Color backColor, float penThickness, int elementCount, int pointSpread, int bufferDistance, int refreshRate)
        {
            box = new PictureBox()
            {
                Name = "box",
                Size = size,
                Location = position,
                BackColor = backColor,
            };

            form.Controls.Add(box);
            pointArraySize = elementCount;
            this.debug = debug;
            this.pointSpread = pointSpread;
            this.thickness = penThickness;

            graphX = box.Width + pointSpread - bufferDistance;

            pointQueue.Add(new PointF(graphX, (float)((size.Height*0.5))));
            for (int i = 0; i < pointArraySize - 1; i++)
                addPoint(generatePoint());

            //Timer stuff for updating the graph
            update = new System.Timers.Timer(refreshRate);
            update.AutoReset = true;
            update.Elapsed += timerUpdate;
            update.Enabled = true;

            box.Paint += paintBox;
        }

        public void updateBox(Size size, Point position, Color backColor)
        {
            if (size != null) 
            { 
                box.Size = size;
                this.boxSize = box.Size;
            }
            if (position != null) 
            { 
                box.Location = position;
                this.boxPosition = new Point(box.Left, box.Top);
            }
            if (backColor != null) { box.BackColor = boxColor = backColor; }
        }
        public Point getBoxLocation() { return boxPosition;  }
        public Size getBoxSize() { return boxSize;  }
        public void addPoint(PointF point)
        {
            pointQueue.Add(point);
            if (pointQueue.Count > pointArraySize)
                _ = pointQueue.Remove();
        }

        //private methods
        private PointF generatePoint()
        {
            return new PointF(graphX, pointQueue.list.Last.Value.Y + debugRandom.Next(-5, 5));
        }
        private void updateGraph()
        {
            var workingNode = pointQueue.list.First;

            for (int i = 0; i < pointArraySize - 1; i++)
            {
                try
                {
                    pointQueue.list.AddBefore(workingNode, new PointF(workingNode.Value.X - pointSpread, workingNode.Value.Y));
                    workingNode = workingNode.Previous;
                    if (workingNode.List.Equals(pointQueue.list) && workingNode.Next.List.Equals(pointQueue.list))
                    {
                        pointQueue.list.Remove(workingNode.Next);
                        workingNode = workingNode.Next;
                    }
                } catch (System.InvalidOperationException)
                {

                }
            }

            box.Invalidate();
        }
        private void timerUpdate(Object sender, ElapsedEventArgs e)
        {
            
            if (debug) addPoint(generatePoint());
            updateGraph();
        }
        private void paintBox(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen test = new Pen(Brushes.DeepSkyBlue);
            test.Width = thickness;
            var workingNode = pointQueue.list.First;

            if (workingNode != null && workingNode.Next != null)
            {
                for (int i = 0; i < pointArraySize - 1; i++)
                {
                    g.DrawLine(test, workingNode.Value, workingNode.Next.Value);
                    workingNode = workingNode.Next;
                }
            }

            test.Dispose();
        }

    }

    class IndexedList<T>
    {
        public LinkedList<T> list;
        public int Count
        {
            get => list.Count;
        }

        public IndexedList()
        {
            list = new LinkedList<T>();
        }

        public void Add(T element)
        {
            list.AddFirst(element);
        }

        public T Remove()
        {
            T returnVal = list.Last.Value;
            list.RemoveLast();
            return returnVal;
        }
    }
}