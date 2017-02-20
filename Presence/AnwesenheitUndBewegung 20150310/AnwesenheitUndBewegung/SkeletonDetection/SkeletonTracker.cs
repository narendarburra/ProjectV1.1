using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace AnwesenheitUndBewegung.SkeletonDetection
{
    class SkeletonTracker
    {
        private object sender;
        private SkeletonFrameReadyEventArgs e;
        private bool hasSkeletons;
        private bool isMoving;
        private Skeleton[] skeletons;
        private Skeleton oldSkeleton;
        private const double ActivityMetricThreshold = 0.05; //TODO Find good Threshold
        private const double DeltaScalingFactor = 10.0;
        private const double ActivityDecay = 0.1;
        private double headActivityLevel;
        private double leftShoulderActivityLevel;
        private double rightShoulderActivityLevel;
        private double positionActivityLevel;

        public SkeletonTracker()
        {
            hasSkeletons = false;
            skeletons = new Skeleton[0];
            isMoving = false;
        }

        public void newSkeleton(object sender, SkeletonFrameReadyEventArgs e)
        {
            this.sender = sender;
            this.e = e;
        }

        public Boolean[] skeletonMovingAndPresent()
        {
            bool[] isPresentAndMoving = new bool[2];
            if(skeletonNotEmpty())
            {
                skeletonMovingChecker();
            }
            else
            {
                hasSkeletons = false;
                isMoving = false;
            }
            isPresentAndMoving[0] = hasSkeletons;
            isPresentAndMoving[1] = isMoving;
            return isPresentAndMoving;
        }

        public bool skeletonNotEmpty()
        {
            bool notEmpty = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }

            }
            if (skeletons.Length != 0)
            {
                notEmpty = true;
            }

            return notEmpty;
        }

        public void skeletonMovingChecker()
        {
            hasSkeletons = false;
            foreach (Skeleton skel in skeletons)
            {
                if (skel.TrackingState == SkeletonTrackingState.Tracked)
                {
                    hasSkeletons = true;

                    if (oldSkeleton != null && oldSkeleton.TrackingState==SkeletonTrackingState.Tracked) 
                    {

                        double headMetric = jointMovementMetric(skel, JointType.Head);

                        this.HeadActivityLevel = ((1.0 - ActivityDecay) * this.HeadActivityLevel) + (ActivityDecay * headMetric);
                        bool isHeadMoving = this.HeadActivityLevel >= ActivityMetricThreshold;

                        if (isHeadMoving) 
                        {
                            isMoving = true;
                        }
                        else
                        {
                            double rightShoulderMetric = jointMovementMetric(skel, JointType.ShoulderRight);
                            this.RightShoulderActivityLevel =((1.0 - ActivityDecay) * this.RightShoulderActivityLevel) + (ActivityDecay * rightShoulderMetric);
                            bool isRightShoulderMoving = this.RightShoulderActivityLevel >= ActivityMetricThreshold;
                            if (isRightShoulderMoving)
                            {
                                isMoving = true;
                            }
                            else
                            {
                                double leftShoulderMetric = jointMovementMetric(skel, JointType.ShoulderLeft);
                                this.LeftShoulderActivityLevel = ((1.0 - ActivityDecay) * this.LeftShoulderActivityLevel) + (ActivityDecay * leftShoulderMetric);
                                bool isLeftShoulderMoving = this.LeftShoulderActivityLevel >= ActivityMetricThreshold;
                                if (isLeftShoulderMoving)
                                {
                                    isMoving = true;
                                }
                                
                                        else
                                        {
                                            isMoving = false;
                                        }

                            }
                        }

                    }
                    oldSkeleton = skel;
                }
                else if(skel.TrackingState == SkeletonTrackingState.PositionOnly)
                {
                    hasSkeletons = true;
                    if (oldSkeleton != null && oldSkeleton.TrackingState==SkeletonTrackingState.PositionOnly)
                    {
                        double newMetric = positionMovementMetric(skel);

                        this.PositionActivityLevel = ((1.0 - ActivityDecay) * this.PositionActivityLevel) + (ActivityDecay * newMetric);
                        bool isPositionMoving = this.PositionActivityLevel >= ActivityMetricThreshold;

                        if (isPositionMoving)
                        {
                            isMoving = true;
                        } 
                    }
                    oldSkeleton = skel;
                }
            }
        }

        double jointMovementMetric(Skeleton skel, JointType joint)
        {
            double deltaX = skel.Joints[joint].Position.X - oldSkeleton.Joints[joint].Position.X;
            double deltaY = skel.Joints[joint].Position.Y - oldSkeleton.Joints[joint].Position.Y;
            double deltaZ = skel.Joints[joint].Position.Z - oldSkeleton.Joints[joint].Position.Z;
            double delta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            return DeltaScalingFactor * delta;
            
        }
        double positionMovementMetric(Skeleton skel)
        {
            double deltaX = skel.Position.X - oldSkeleton.Position.X;
            double deltaY = skel.Position.Y - oldSkeleton.Position.Y;
            double deltaZ = skel.Position.Z - oldSkeleton.Position.Z;
            double delta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            return DeltaScalingFactor * delta;

        }

        public double HeadActivityLevel
        {
            get
            {
                return this.headActivityLevel;
            }

            private set
            {
                this.headActivityLevel = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        public double LeftShoulderActivityLevel
        {
            get
            {
                return this.leftShoulderActivityLevel;
            }

            private set
            {
                this.leftShoulderActivityLevel = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        public double RightShoulderActivityLevel
        {
            get
            {
                return this.rightShoulderActivityLevel;
            }

            private set
            {
                this.rightShoulderActivityLevel = Math.Max(0.0, Math.Min(1.0, value));
            }
        }

       
        public double PositionActivityLevel
        {
            get
            {
                return this.positionActivityLevel;
            }

            private set
            {
                this.positionActivityLevel = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
    }
}
