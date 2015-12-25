using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

public static class FractionalLineAlgorithm
{
    /// <summary>
    /// Function for tracing a line. Note: Reasonably deterministic - safe to use with LSF simulation.
    /// </summary>
    /// <param name="startX">Start x.</param>
    /// <param name="startY">Start y.</param>
    /// <param name="endX">End x.</param>
    /// <param name="endY">End y.</param>
    public static IEnumerable<Coordinate> Trace(double startX, double startY, double endX, double endY)
    {
        //TODO: Make it look prettier

        const double one = 1;
        const double half = .5d;

        double deltaX = endX - startX;
        double deltaY = endY - startY;
        double absDeltaX = Math.Abs(deltaX);
        double absDeltaY = Math.Abs(deltaY);
        int directionX = Math.Sign(deltaX);
        int directionY = Math.Sign(deltaY);
        int gridX = (int)Math.Round(startX);
        int gridY = (int)Math.Round(startY);
        yield return new Coordinate(gridX, gridY);

        if (deltaX == 0)
        {
            if (deltaY == 0)
            {
                yield break;
            }
            //Vertical
            yield break;
        }
        if (deltaY == 0)
        {
            //Horizontal
            yield break;
        }

        double positionX = startX;
        double positionY = startY;
        double slope = Math.Abs(deltaY / deltaX);

        double lastChangePosition = positionY;

        double used = 0d;

        //Getting X to align with a vertical edge of the grid... easier to calculate coordinates
        if (positionX % half != 0 || positionX % 1 == 0)
        {
            double newPositionX = directionX > 0 ? Math.Round(positionX) + .5f : Math.Round(positionX) - .5f;
            double difX = newPositionX - positionX;

            double absDifX = Math.Abs(difX);
            double difY = absDifX * slope * directionY;
            double newPositionY = positionY + difY;
            if (
                (directionY > 0 && newPositionY > endY) ||
                (directionY < 0 && newPositionY < endY))
            {
                newPositionY = endY;
            }
            else {
                double lastChangeDif = Math.Abs(newPositionY - lastChangePosition);

                bool yPassed = false;
                if (lastChangeDif < one)
                {
                    yield return new Coordinate(gridX, gridY);

                    double roundDirection = directionY > 0 ? half : -half;
                    double compare = Math.Abs(Math.Round(lastChangePosition) + roundDirection - lastChangePosition);
                    if (lastChangeDif > compare)
                    {
                        yPassed = true;
                        gridY += directionY;
                        yield return new Coordinate(gridX, gridY);

                    }

                } else
                {
                    yPassed = true;
                    for (double y = 0d; ; y += one)
                    {
                        if (y + 1 > lastChangeDif) {
                            double roundDirection = directionY > 0 ? half : -half;
                            lastChangeDif -= y;
                            double compare = Math.Abs(Math.Round(lastChangePosition) + roundDirection - lastChangePosition);
                            if (lastChangeDif > compare)
                            {
                                yPassed = true;
                                gridY += directionY;
                                yield return new Coordinate(gridX, gridY);

                            }
                            break;
                        }
                        gridY += directionY;
                        yield return new Coordinate(gridX, gridY);

                        if (y + 1 == lastChangeDif) break;

                    }
                }

                if (yPassed)
                {
                    lastChangePosition = newPositionY;
                }
            }
            gridX += directionX;
            positionX = newPositionX;
            positionY = newPositionY;
        }

        bool doBreak = false;
        for (double x = used;; x += one)
        {
            double difX = directionX;
            if (x + one >= absDeltaX)
            {
                difX = absDeltaX - x;
                doBreak = true;
            }
            double newPositionX = positionX + difX;

            double absDifX = Math.Abs(difX);
            double difY = absDifX * slope * directionY;
            double newPositionY = positionY + difY;

            if (
                (directionY > 0 && newPositionY > endY) ||
                (directionY < 0 && newPositionY < endY))
            {
                newPositionY = endY;
            }
            double lastChangeDif = Math.Abs(newPositionY - lastChangePosition);
            bool yPassed = false;
            yield return new Coordinate(gridX, gridY);

            if (lastChangeDif < one)
            {

                double roundDirection = directionY > 0 ? half : -half;
                double compare = Math.Abs(Math.Round(lastChangePosition) + roundDirection - lastChangePosition);
                if (lastChangeDif > compare)
                {
                    yPassed = true;
                    gridY += directionY;
                    yield return new Coordinate(gridX, gridY);
                }

            } else
            {
                yPassed = true;
                for (double y = 0d; ; y += one)
                {
                    if (y + 1 > lastChangeDif) {
                        double roundDirection = directionY > 0 ? half : -half;
                        lastChangeDif -= y;
                        double compare = Math.Abs(Math.Round(lastChangePosition) + roundDirection - lastChangePosition);
                        if (lastChangeDif > compare)
                        {
                            yPassed = true;
                            gridY += directionY;
                            yield return new Coordinate(gridX, gridY);

                        }
                        break;
                    }
                    gridY += directionY;
                    yield return new Coordinate(gridX, gridY);

                    if (y + 1 == lastChangeDif) break;

                }
            }

            if (yPassed)
            {
                lastChangePosition = newPositionY;
            }
            if (doBreak)
                break;
            

            gridX += directionX;
            positionX = newPositionX;
            positionY = newPositionY;


        }
    }

    public struct Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }

}
