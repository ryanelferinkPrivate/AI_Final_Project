# Hide_and_Seek

Hi all, this is my first experience using GitHub with Unity, so we may run into some issues as we go.

**Setting up your environment**

You should first clone the repo into a directory you wish to use with Unity. You should now have a directory which has folders such as Assets, Packages, etc. Then create a default 3D Unity project through Unity Hub and set the working directory to the directory you have cloned the repo as (the one containing Assests, Packages, etc.). This should (hopefully) allow you to push and pull in that directory, updating the Unity workspace.

Let me know if you any of this doesn't make sense and I'll do my best to help.

**This is how you should push to the repo after making changes in Unity (though I have not tested this with anyone yet)**

git add Assets ProjectSettings Packages    <-- NOTE: Make sure you use this format and not git add . as we don't want every file

git commit -m "Describe what changed (e.g. updated enemy AI, new scene)"

git push origin main

**Pull instructions**

git pull origin main

If you run into any issues, let me know
- Ethan
