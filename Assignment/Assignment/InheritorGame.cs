using System;
using System.Collections.Generic;

/// <summary>
/// Creates the rooms, transitions, and items for this adventure game and connects them.
/// </summary>
public class InheritorGame : Game
{
    const string NORTH = "north";
    const string SOUTH = "south";
    const string EAST = "east";
    const string WEST = "west";
    const string UP = "up";
    const string DOWN = "down";

    // outside the house
    Room outside = new Room { RoomDescription = "You decide to take what you've already found and leave. Something about this place unnerves you, and you never return.", Items = new List<Item>(), FinalRoom = true, RoomName = "outside" };
    // entrance hall
    Room entranceHall = new Room { RoomDescription = "You are in the entrance hall. There is a door leading further into the house to the north, and an exit to the south.", Items = new List<Item>(), FinalRoom = false, RoomName = "entrance hall" };
    // living room
    Room livingRoom = new Room
    {
        RoomDescription = "You are in the living room. There are doors to the north, south, east, and west. There is a staircase going down.",
        Items = new List<Item> {
        new Item { Name = "trophy case", Description = "A trophy case containing a massive golden cup", PointValue = 150 },
        //new Item { Name = "elven sword", Description = "A leaf-bladed longsword, elven crafted.", PointValue = 150 },
        new WeaponItem { Name = "elven sword", Description = "A leaf-bladed longsword, elven crafted.", PointValue = 150, IsAWeapon=true },
        new Item { Name = "fancy rug", Description = "A large, oriental-style rug with exceptional craftsmanship.", PointValue = 100 },
        },
        RoomName = "living room",
        FinalRoom = false
    };
    // painting room
    Room paintingRoom = new Room
    {
        RoomDescription = "You are in the painting room. There is a Harpsichord. A painting depicts a skeleton holding open a gateway to an underground passage. A male elf is entering the passage. A female elf is holding a strange orb. A human man stands to the side observing.",
        Items = new List<Item> {
        //new Item { Name = "harpsichord", Description = "An incredibly heavy harpsichord.", PointValue = 300},
        new AttackableItem { Name = "harpsichord", Description = "An incredibly heavy harpsichord.", PointValue = 300, IsDestroyed = false,
                    NewItem = new Item{ Name = "gold", Description = "Shiny gold coins.", PointValue = 100 } },
        new Item { Name = "oil painting", Description = "The painting depicts a skeleton holding open a gateway to an underground passage. A male elf is entering the passage. A female elf is holding a strange orb. A human man stands to the side observing.", PointValue = 150 },
    },
        RoomName = "painting room",
        FinalRoom = false
    };
    // kitchen
    Room kitchen = new Room
    {
        RoomDescription = "You are in the kitchen. A table seems to have been used recently for the preparation of food. A passage leads to the west.",
        Items = new List<Item> {
        new Item { Name = "sack of peppers", Description = "A brown sack containing spicy green peppers.", PointValue = 1 },
        new Item { Name = "glass of water", Description = "A refreshing glass of cold water.", PointValue = 1 },
    },
        RoomName = "kitchen",
        FinalRoom = false
    };
    // fancy bedroom
    Room fancyBedroom = new Room
    {
        RoomDescription = "You are in the fancy bedroom. There is a four-poster bed with red sheets. There is a closed chest at the foot of the bed.",
        Items = new List<Item> {
        new Item { Name = "boots", Description = "Tough boots with spikes for climbing.", PointValue = 10 },
        new Item { Name = "sheets", Description = "Fancy silk sheets", PointValue = 50 },
        new Item { Name = "gold", Description = "Shiny gold coins.", PointValue = 100 },
    },
        RoomName = "fancy bedroom",
        FinalRoom = false
    };
    // cellar
    Room cellar = new Room
    {
        RoomDescription = "You are in a tidy cellar. There are barrels of wine here. A door leads to the north, and a staircase goes up.",
        Items = new List<Item>{
        new Item { Name = "wine", Description = "A bottle of fine wine.", PointValue = 50 },
    },
        RoomName = "cellar",
        FinalRoom = false
    };
    // library
    Room library = new Room
    {
        RoomDescription = "You are in a large library. There are many books about anatomy, history, and alchemy. Some of the books are written in Elven. There is a door to the south.",
        Items = new List<Item> {
        new Item { Name = "necklace", Description = "A ruby necklace.", PointValue = 125 },
        new Item { Name = "elven book", Description = "A tome written in the indecipherable elven dialect.", PointValue = 100 },
    },
        RoomName = "library",
        FinalRoom = false
    };
    // laboratory
    Room laboratory = new Room
    {
        RoomDescription = "You find yourself in a strange laboratory. A lamp with a red filter lights the room. There is a secret passage to the south. There is a door with a skull to the north.",
        Items = new List<Item> {
        new Item { Name = "orb", Description = "The Orb of Yendor, an ancient artifact that has been missing for many years.", PointValue = 500 },
        new Item { Name = "flask", Description = "A flask encrusted with gems.", PointValue = 200 },
        new Item { Name = "lamp", Description = "A lamp with a ruby-tinted filter.", PointValue = 30 },
    },
        RoomName = "laboratory",
        FinalRoom = false
    };
    // skeleton room
    Room skeletonRoom = new Room
    {
        RoomDescription = "The strange door opens into darkness. You peer in, and a pair of skeletal hands reach out and drags you in! The last thing you see is a strange underground passage before the last of the light disappears.",
        Items = new List<Item>(),
        FinalRoom = true,
        RoomName = "skeleton room"
    };

    // enemy Troll
    Enemy troll = new Enemy
    {
        Name = "Troll",
        IsDestroyed = false,
        NewItem = null,
        Description = "An ugly creature depicted as either a giant or a dwarf",
        PointValue = 250,
        inventoryFlg = false
    }; 


    public void SetupRooms()
    {
        entranceHall.Transitions = new Dictionary<string, Room>
        {
            {SOUTH, outside},
            {NORTH, livingRoom},
        };

        livingRoom.Transitions = new Dictionary<string, Room>
        {
            {SOUTH, entranceHall},
            {NORTH, fancyBedroom},
            {EAST, kitchen},
            {WEST, paintingRoom},
            {DOWN, cellar},
        };
        // fancy bedroom
        fancyBedroom.Transitions = new Dictionary<string, Room>
        {
            { SOUTH, livingRoom},
        };
        // kitchen
        kitchen.Transitions = new Dictionary<string, Room>
        {
            { WEST, livingRoom},
        };
        // painting room
        paintingRoom.Transitions = new Dictionary<string, Room>
        {
            { EAST, livingRoom},
        };
        // cellar
        cellar.Transitions = new Dictionary<string, Room>
        {
            { UP, livingRoom},
            { NORTH, library},
        };
        // library
        library.Transitions = new Dictionary<string, Room>
        {
            { "pick up elven book", laboratory},
            { SOUTH, cellar},
        };
        // laboratory
        laboratory.Transitions = new Dictionary<string, Room>
        {
            { NORTH, skeletonRoom},
            { SOUTH, library},
        };

        //Troll
        troll.enemyCurrentRoom = laboratory;
        enemies.Add(troll);

    }

    public InheritorGame()
    {
        currentRoom = entranceHall;
        
        introduction = "Weeks ago, you received a mysterious letter claiming that your late grandfather (who you don't know anything about) left you his house and land in the mountains. Having no property yourself, this is a substantial inheritance. After a few days of hiking into the countryside, you come upon the house, opulent and imperial, standing proudly against the hills leading into the mountain behind it.";
        SetupRooms();
    }
}
