1:59 PM 4/27/2018

For Unity forum post: https://forum.unity.com/threads/confused-about-rotations.528673/

I apologize for oversimplifying in my first post above.

The actual solution was more complicated than I realized, but the ball does not move in my solution.

- attitude indicator ball fixed in space (never rotates)

- a common camera rig with two cameras parented below it:
	- main camera facing +Z the way you would expect it
	- AI camera facing -Z basically 180 degrees different

Use layers to make the AI camera only see the AI ball, and the main camera see everything else.

The above gets you Yaw and Pitch correct but Roll is the wrong way.

Therefore I wrote a script to drive the AI camera anti-Z (TrackAntiZ.cs).

When you turn the direction of the camera rig, the AI camera will show the "correct" side of the AI Ball.

See onscreen instructions. It is still subject to gimbal lock because I am tracking Yaw/Pitch/Roll interdependent of the resultant rotation.

See attached project. That attitude texture is the first one off Google Images, with E/W reversed.

Kurt Dekker
www.twitter.com/kurtdekker
