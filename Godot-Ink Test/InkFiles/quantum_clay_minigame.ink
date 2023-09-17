INCLUDE quantum_clay_minigame_functions.ink

->start
==start
TEXT_AMOUNT({daysLeft})
+ [Begin]
->intro

==intro
{SetBackground(Campfire)}
At long last, you and your [b]wercru[/b] has arrived at the coordinates given to you by [b]Tipak the Shaman[/b]. Somewhere in the ruins around here you should be able to find what you need: functional [b]Claywork devices[/b] from which the Shamans can harvest [b]residua[/b].

Residua, in turn, can be used to answer an important question: [b]what is Quantum Motion?[/b]

+ [Go to map]
->map.enter

==map
=enter
{GoToMap()}
Select where you want to go from among the available locations.

Note that searching a location costs [b]Energy[/b]. Once you reach 0 Energy, you will have to return to Camp to rest for the day.

You have rations for {daysLeft} days. Once your rations run out, you need to head back home...

+ [Back to camp {CustomButton("camp")}]
->exit
+ [Ruins (Cost: 2) {CustomButton("ruin_1")} INTERACTABLE({energy>=2})]
{AlterEnergy(-2)}
->ruins_1
+ [Ruins (Cost: 3) {CustomButton("ruin_2")} INTERACTABLE({energy>=3})]
{AlterEnergy(-3)}
->ruins_2

=exit
MAP(-1)
->camp


=ruins_1
{SetBackground(Ruins)}
You enter the ruins.
* [Loot]
{AddItem(CrackedPhone)}
->ruins_1
+ [Leave]
->map.enter
=ruins_2
{SetBackground(Ruins)}
You enter the other ruins.
* [Loot]
{AddItem(CrackedTablet)}
->ruins_2
+ [Leave]
->map.enter


==camp
{SetBackground(Campfire)}
You return to camp!

+ {LIST_COUNT(allItems)>0} [Extract Residua]
->ExtractResidua

+ {energy<10} [Rest]
~energy = 10
{AlterEnergy(0)}
{AlterRations(-1)}
You rest all night and are now energized once more!
->camp

+ [To map]
->map.enter

=ExtractResidua
+ {allItems?CrackedPhone} [Open Small Black Mirror]
{AddItem(Residua)}
~allItems-=CrackedPhone
You break open the small Black Mirror and extract the Residua.

+ {allItems?CrackedTablet} [Open Large Black Mirror]
{AddItem(Residua)}
~allItems-=CrackedTablet
You break open the large Black Mirror and extract the Residua.

+ [Finish]
You finish.
->camp

- ->ExtractResidua

==test

Once upon a time... PORTRAIT(-1) BACKGROUND(2) MAP(0)

 + There were two choices.
 This choice had a lot of text in it, enough text in fact inside a single bubble to force multiple lines. How, I wonder, was Godot going to handle it?
 
 Poorly? Not at all? Or very very well? Who could tell, in the end? Let's see how it all plays out, eh.
 PORTRAIT(0) PLAY_ANIMATION(fade_out) BACKGROUND(0)
 + There were four lines of content.
 PORTRAIT(1) PLAY_MUSIC(0) PLAY_ANIMATION(fade_in) BACKGROUND(1)

+ [Ruin 1 Click BUTTONTAG(ruin_1) INTERACTABLE({energy>0})]
Clicked ruin 1! BACKGROUND(1) MAP(-1) ITEM_FOUND(testItem)
~energy--
SLIDER_ENERGY({energy})
+ [Ruin 2 click BUTTONTAG(ruin_2) INTERACTABLE({energy>=2})]
Clicked ruin 2! BACKGROUND(0) MAP(-1) ITEM_FOUND(testItem2)
~energy-=2
SLIDER_ENERGY({energy})
+ [Camp BUTTONTAG(camp) INTERACTABLE({energy<=0})]
You went to camp! And got all your energy back, yaaay! Also lost food oh no.

~energy = 10
~daysLeft--
TEXT_AMOUNT({daysLeft})
SLIDER_ENERGY({energy})
- They lived happily ever after.
+ [End]
    -> start
