## Adding the Portal Gun to the Player (Capsule)

Follow these steps to add the Portal Gun component to your Unity project:

### Step 1: Create the Portal Gun Object and Add the Script as a component
1. Go to `Hierarchy`, select 3D object and choose Cube, and reset it.
2. Drag the object to become a child of the Player Capsule
3. Scale the cube to `X:0.1, Y:0.1, Z:1` and move it around so that its visible in the Camera View
4. Now add the `PortalGun.cs` to your Gun object and make sure it is called `PortalGun.cs`. Drag the main camera to the `Player Camera` option in the script and follow the steps for the `Portal Prefab A/B` objects

### Step 2: Creating Portal Prefab A/B Objects
1. Go to `Hierarchy`, select 3D object and choose Cube. (Reseting does not matter as these objects are just representing the portal. It is recommended to hide it from your view by just moving it out of the way)
2. Rename the Object to `Portal A` and click on it. After clicking on it , Go to `Inspecter View (Top Right)` and make sure that the `Tag` says `Portal`. If it does not say `Portal` then `Add Tag` , Press `+` and create the `Portal` Tag.
3. Scale the Portal to `X:1, Y:2, Z:0.1` and add a `Box Collider` as a Component. 
4. Make sure that the `Is Trigger` option is ticked inside the Box Collider component.
5. Change the `Center` of `Box Collider` to `X:0, Y:0, Z:0` and the `Size` of `Box Collider` to `X:1, Y:1, Z:4.6`
6. Add the `Portal.cs` component to the `Portal A` Object.
7. `Duplicate` The `Portal A` Object and rename the Duplicated Object `Portal B`, and make sure that it has the same `Scale, Box Collider, Portal.cs` options as `Portal A`. 
8. Now Select Both `Portal A & Portal B` objects in the `Hierarchy View` and drag them to the `Assets folder` in the bottom of the screen. This will make them `Prefab Objects`
9. Now Click on `Portal A` and drag the `Portal B Prefab Object from the Hierarchy view` to the `Linked Portal Section` in the `Portal.cs Script` , and do the same with `Portal B` so that `Portal B's` linked portal is `Portal A`.
10. Finally Click on the `Portal Gun Object` you created in Step 1 and drag `Both Portal A & B Objects from the Hierarchy View to their respective Portal Prefab Objects, one by one` 

### Step 3: Creating the CrossHair
1. Click the `+` on `Hierarchy View` and Go to `UI -> Canvas` and make sure that `Render Mode` is set to `Screen Space - Overlay` in Canvas's options
2. Right Click `Canvas` in the `Hierarchy View` and go to `UI -> Image`. You can rename the Image if you want , it doesn't matter. 
3. Click on `Image` and in the `Inspector View`, Under the `Rect Transform` Change the `Width & Height to 10` and on the Left side of 
`Rect Transform` make sure that the `Anchor Presets (Grid Like Image)` is at `Center Middle`
4. Click on `Source Image` in the `Inspector View` and change it to `UISprite`. You can also change the color by clicking on it as it will be white by default
5. Ignore the `EventSystem` Object which is created with the Canvas as I have no idea if its useful 

### Step 4: Add the Player Tag to Player Capsule
1. Click on Player Capsule and make sure that the `Tag` is set to `Player` . If there is no Player Tag, then create it the using the same steps in `Step 2` and assign it to the Player Capsule
2. Get the Code for `PortalGun.cs` & `Portal.cs` from this branch and ignore the .meta files 
3. After doing all of this, The portal gun should work ! 

