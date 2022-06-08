Woodway Treadmill Unity Package

Software requirements: 
-	Compatible with Unity 2021.30f1

Install:
-	Download package
-	Copy PARI folder from _Trinoma/InstallFiles to C:\Users\Public
-	Make sure Public folder is shared 
-	In the Project or Build folder:
-	Make sure to always put the 2 xml files:
	-	PARIConfig.xml: configuration settings for monitors and projectors
	-	Scene_config.xml: configuration settings for display
-	In Unity, set the following parameters in Project Settings:
	-	Time : these settings are due to COM port communication method
		-	Fixed Timestep : 0.3
		-	Maximum Allowed Timestep : 0.3
		-	Time Scale : 1
		-	Maximum Particle Timestep : 0.3
	-	Player: .NET Framework
	

Note: The PARIConfig file is monitor configuration dependent and can be modified for testing on a different equipment. The XML file associated to the build on the customer operating computer must NOT change. Those files are not generated during Build and must be copied from another project. 


How to use package: Example scene in _Trinoma/Scenes/TreadmillScene.unity
-	CanvasOperateur: contains all the visual objects operator can interact with
	-	TreadmillGUI: treadmill controls
		-	Treadmill GUI Config script: must be linked to TreadmillController Object: all other linked are then configured automatically
		-	Contains Sliders and buttons for treadmill controls
-	PARIManager: Projection warping Handler (DO NOT MODIFY)
-	RootPlayer: Main game object
	-	Camera Controller script: drives camera and ground positions and displacements
	-	MainCamera: main camera object
-	TreadmillController: contains scripts driving reading from and writing to treadmill (NOT TO MODIFY)
	-	TreadmillGUI Link script: must be linked to the Treadmill GUI object to connect with control sliders and buttons (reference is done automatically at start after linking the treadmillGUI object)
-	Ground: ground prefab used in the loop of the RootPlayer
-	TreadmillDevControls: object to facilitate access to read and write values from and to treadmill in scripts
	-	Treadmill Dev Controls script: must be linked to TreadmillController object
-	Speed and angle values are written in mainSpeed and mainAngle FloatVariables. 
	-	These FloatVariables are used communicate between TreadmillController and CameraController to get the speed and angle value to follow. The objects are made to facilitate developments in case speed or angle needs to be driven by other scripts. 