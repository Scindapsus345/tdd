﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TagsCloudVisualization
{
    class CircularCloudLayouter
    {
        private readonly Point _center;
        private List<Rectangle> rectangles;
        private int cloudRadius;

        public CircularCloudLayouter(Point center)
        {
            if (center.X < 0 || center.Y < 0)
                throw new ArgumentException();
            _center = center;
            rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException();
            var radius = Math.Max((double)cloudRadius-10, 0);
            var angle = 0d;
            var radiusStep = 0.02;
            var angleStep = 0.3;
            while (true)
            {
                var currentCoordinate = new Point(
                    (int)(_center.X + radius * Math.Cos(angle) - rectangleSize.Width / 2),
                    (int)(_center.Y - radius * Math.Sin(angle)) - rectangleSize.Height / 2);
                var currentRectangle = new Rectangle(currentCoordinate, rectangleSize);

                if (!CheckIntersections(currentRectangle))
                {
                    currentRectangle = ShiftRectangleToCenter(currentRectangle);
                    cloudRadius = Math.Max(currentRectangle.Right - _center.X, cloudRadius);
                    rectangles.Add(currentRectangle);
                    break;
                }

                radius += radiusStep;
                angle += angleStep;
            }

            return rectangles.Last();
        }

        private Rectangle ShiftRectangleToCenter(Rectangle rectangle)
        {
            var step = 1;
            var availablePosition = rectangle.Location;
            while (true)
            {
                var newPosition = new Point(availablePosition.X, availablePosition.Y);
                ShiftHorizontal(rectangle, availablePosition, newPosition, step);
                ShiftVertical(rectangle, availablePosition, newPosition, step);

                if (availablePosition == newPosition)
                    break;
                availablePosition = newPosition;
            }
            return new Rectangle(availablePosition, rectangle.Size);
        }

        private void ShiftVertical(Rectangle rectangle, Point availablePosition, Point newPosition, int step)
        {
            if (Math.Abs(availablePosition.Y - _center.Y + rectangle.Height / 2) > 1)
            {
                newPosition.Y = availablePosition.Y >= _center.Y - rectangle.Height / 2
                    ? availablePosition.Y - step
                    : availablePosition.Y + step;
                if (CheckIntersections(new Rectangle(newPosition, rectangle.Size)))
                    newPosition.Y = availablePosition.Y;
            }
        }

        private void ShiftHorizontal(Rectangle rectangle, Point availablePosition, Point newPosition, int step)
        {
            if (Math.Abs(availablePosition.X - _center.X + rectangle.Width / 2) > 1)
            {
                newPosition.X = availablePosition.X >= _center.X - rectangle.Width / 2
                    ? availablePosition.X - step
                    : availablePosition.X + step;
                if (CheckIntersections(new Rectangle(newPosition, rectangle.Size)))
                    newPosition.X = availablePosition.X;
            }
        }

        private bool CheckIntersections(Rectangle rectangle)
        { 
            foreach (var otherRectangle in rectangles)
                if (rectangle.IntersectsWith(otherRectangle))
                    return true; 

            return false;
        }

        public List<Rectangle> GetRectangles() => rectangles;
    }
}