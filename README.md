# PhyloFun
 Unity game based on phylogenetic rearrangement problems


### Building

To build for WebGL, play with `hudRightMargin` in `ScreenUtils` to get all things on the screen. For example, set it dynamically: to `300f/1280f*Screen.width`. The screen width seems to be 300, so about 70 should work. For all other builds, set it to `300f`? If this does not set the game part of the screen correctly for webGL, then it's also possible to fix this slightly by playing with the resolution settings of the WebGL build in index.html. 