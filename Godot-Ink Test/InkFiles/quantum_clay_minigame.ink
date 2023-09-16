->start
==start

+ [Start game]
->test
==test

Once upon a time... PORTRAIT(-1) BACKGROUND(2)

 + There were two choices.
 This choice had a lot of text in it, enough text in fact inside a single bubble to force multiple lines. How, I wonder, was Godot going to handle it?
 
 Poorly? Not at all? Or very very well? Who could tell, in the end? Let's see how it all plays out, eh.
 PORTRAIT(0) PLAY_ANIMATION(fade_out) BACKGROUND(0)
 + There were four lines of content.
 PORTRAIT(1) PLAY_MUSIC(0) PLAY_ANIMATION(fade_in) BACKGROUND(1)

+ [Ruin 1 Click BUTTONTAG(ruin_1)]
Clicked ruin 1!
+ [Ruin 2 click BUTTONTAG(ruin_2)]
Clicked ruin 2!

- They lived happily ever after.
+ [End]
    -> start
