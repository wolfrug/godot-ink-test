// Functions file

VAR energy = 10
VAR daysLeft = 5
VAR residua = 0
VAR clay = 0

LIST characters = Shaman, Storyteller
LIST backgrounds = Campfire, Ruins, Black, Forest
LIST allItems = CrackedPhone, CrackedTablet, Rations, Residua, Clay

===function SetPortrait(portrait)===
{portrait:
- Shaman:
PORTRAIT(0)
-Storyteller:
PORTRAIT(1)
}
===function SetBackground(background)===
MAP(-1)
{background:
- Campfire:
BACKGROUND(1) PLAY_AMBIENCE(0)
- Ruins:
BACKGROUND(0) PLAY_AMBIENCE(2)
- Forest:
BACKGROUND(3) PLAY_AMBIENCE(1)
- Black:
BACKGROUND(2) PLAY_AMBIENCE(-1)
}
===function GoToMap()===
BACKGROUND(-1) MAP(0) PLAY_AMBIENCE(1) PORTRAIT(-1)
===function CustomButton(tag)
BUTTONTAG({tag})

===function AddItem(item)
{not (allItems?item):
~allItems+=item
{item:
-CrackedPhone:
ITEM_FOUND(CrackedPhone)
-CrackedTablet:
ITEM_FOUND(CrackedTablet)
-Rations:
ITEM_FOUND(Rations)
{AlterRations(1)}
-Residua:
ITEM_FOUND(Residua)
~residua++
-Clay:
ITEM_FOUND(Clay)
~clay++
}
}

===function AlterRations(number)
{daysLeft+number>=0:
~daysLeft+=number
- else:
~daysLeft = 0
}
TEXT_AMOUNT({daysLeft})

===function AlterEnergy(number)
{energy+number>=0:
~energy+=number
-else:
~energy = 0
}
SLIDER_ENERGY({energy})
