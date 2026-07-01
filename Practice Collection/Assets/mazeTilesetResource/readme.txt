Thank you for purchasing the Maze Tileset Resource!

This package contains 16 tileable meshes that can be used to build a maze.


All meshes are UVW-unwrapped and can be easily retextured using the template in the "documentation" folder.

Short descriptions of the templates' parts:

A: the texture for the maze's wall
B: the texture for the ground
C: the texture for the the corners (aka columns), the right hand side of the texture will be the corner, the left hand side will collide with the wall texture
D: the texture for the top side of the walls
E: the texture for the meshes maze_ceiling and maze_wall

You might want to give the background a color similar to the one of your texture to avoid black borders.


The picture "documentation/description.png" shows all meshes in the package from above, the black parts of the icons are walls, the white parts are ground


The picture "documentation/description2.png" shows how to use maze_ceiling to cover the ceiling of the maze and how to use maze_wall to prohibit the player from looking into the void through the holes above the walls.
You should place one maze_ceiling at the position of every maze tile and create a border around your whole maze using maze_wall.


To build your maze properly, you should use Unity's snap function.
The recommended Snap Settings to create a maze with unscaled tiles are:

Move X: 1.12 or 2.24
Move Y: 1.12 or 2.24
Move Z: 1.12 or 2.24
Scale: whatever you want
Rotation: 45° or 90°


Hints:
-You activate the Snap-function by holding ctrl while moving or rotating an object.
-Use ctrl+d to duplicate an existing tile.
-Create a parent-object for your maze by creating a primitive an deleting all of its components except the Transform-component. Make all your maze-tiles children of this parent object so you can move/rotate/scale the whole maze by manipulating the parent.
-Create your own texture for your maze. The texture contained in this package is only an example and will propably not meet your quality standards.


Changes 1.1:
-added collision to all maze meshes
-scaled all meshes by a factor of 7, so the maze's size fits Unity's standard (measured on the size of the 3rd person controller standard asset)


If there are any problems with this package, feel free to ask for support in the comment section of the asset store.