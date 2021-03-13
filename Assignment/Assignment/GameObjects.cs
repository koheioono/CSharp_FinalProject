using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

/**
 * This file contains class definitions for common game objects for the text-style
 * adventure we're writing.
 */

/// <summary>
/// A scoreboard object that can be used to tabulate a final score.
/// </summary>
[Serializable]
public class Scoreboard
{
    public uint Score { get; set; }
}


// =================Assignment===================================


/// <summary>
/// An attackable item that creates a new item when IsDestroyed is true
/// </summary>
[XmlInclude(typeof(AttackableItem))]
[XmlInclude(typeof(Enemy))]
public interface IAttackable
{
    // you decide what members IAttackable should have
    
    public bool IsDestroyed { get; set; }
    public Item NewItem { get; set; }
}

/// <summary>
/// A weapon to attack an enemy
/// </summary>
[XmlInclude(typeof(WeaponItem))]
public interface IWeapon
{
    // you decide what members IWeapon should have
    public bool IsAWeapon { get; set; }
    public int Attack(IAttackable attackable);
}



/// <summary>
/// An item that can be picked up and added to inventory.
/// </summary>
[Serializable]
public class Item
{
    /// <summary>
    /// The short name used when the item is displayed in lists, etc.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The long description printed from the `describe` command.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gold value used when score is calculated
    /// </summary>
    public uint PointValue { get; set; }

    /// <summary>
    /// Pretty-print the item for display in inventory and rooms.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Name}: ({PointValue} gold) {Description}";
    }

}

[Serializable]
public class AttackableItem : Item, IAttackable
{
    
    public bool IsDestroyed { get; set; } = false;
    public Item NewItem { get; set; }
}

[Serializable]
[XmlInclude(typeof(IWeapon))]
public class WeaponItem : Item, IWeapon
{
    public bool IsAWeapon { get; set; }
    public int Attack (IAttackable attackable)
    {
        attackable.IsDestroyed = true;

        if ((attackable as Enemy) != null){
            Console.WriteLine($"{(attackable as Enemy).Name} is struck with {this.Name}");

            // 1: Enemy
            return 1;
            
        } else if ((attackable as AttackableItem) != null) {
            // 2: AttackableItem
            return 2;
        } else
        {
            // 0: neither of them
            return 0;
        }
        
    }
}

/// <summary>
/// Enemy class
/// </summary>
[Serializable]
public class Enemy : Item, IAttackable
{
    //public String Name { get; set; }
    //public string Description { get; set; }
    public bool IsDestroyed { get; set; } = false;
    public Room enemyCurrentRoom { get; set; }
    public Item NewItem { get; set; }
    public String roomName { get; set; }
    public bool inventoryFlg { get; set; } = false;
}

[Serializable]
public class Room
{
    /// <summary>
    /// If true, the game should end when this room is entered.
    /// </summary>
    public bool FinalRoom { get; set; }
    public string RoomDescription { get; set; }

    public String RoomName { get; set; }

    // initialize to empty list
    public List<Item> Items { get; set; }

    // initialize to an empty dict
    [XmlIgnore]
    public Dictionary<string, Room> Transitions { get; set; }

    public void PrintRoom(List<Enemy> enemies = null)
    {
        Console.WriteLine(this.RoomDescription);
        
        if (Items.Count > 0)
        {
            foreach (Item item in Items)
            {
                if ((item as IAttackable) != null && (item as IAttackable).IsDestroyed == true)
                {
                    Console.WriteLine($"A destroyed {item.Name}, worth 0.");
                } else
                {
                    Console.WriteLine(item);//invokes Item.ToString()
                }
            }
        }

        if (enemies != null && enemies.Count > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                if (this.Equals(enemy.enemyCurrentRoom))
                {
                    if (!enemy.IsDestroyed)
                    {
                        Console.WriteLine($"{enemy.Name} is in the room.");
                    }
                    else
                    {
                        Console.WriteLine($"{enemy.Name} lies destroyed here.");
                    }

                }
            }
            
        }
        
    }
}

[Serializable]
public class Game
{
    //[XmlElement("Scoreboard")]
    public Scoreboard scoreboard = new Scoreboard();
    //[XmlElement("currentRoom")]
    public Room currentRoom;
    public string introduction = "";
    //[XmlArray("inventory")]
    //[XmlArrayItem("item")]
    public List<Item> inventory = new List<Item>();
    //[XmlArray("enemies")]
    //[XmlArrayItem("enemy")]
    public List<Enemy> enemies = new List<Enemy>();
    public int itemFlg = 0;
    public bool validAction = false;
    public int moveCount = 0;
    public bool enemyMoveFlg = false;
    public bool slayedFlg = false;
    public bool finishFlg = false;
    


    public Game() { }

    public void StartGame()
    {
        Console.WriteLine(introduction);
        InputLoop();
        PrintScore();
    }

    public void InputLoop()
    {
        while (true)
        {
            validAction = false;
            itemFlg = 0;
            

            if (moveCount == 1 || enemyMoveFlg)
            {
                foreach(Enemy enemy in enemies)
                {
                    if (!enemy.IsDestroyed)
                    {
                        Console.WriteLine($"{enemy.Name} is now in {enemy.enemyCurrentRoom.RoomName}.");
                    } 
                }
            }

            currentRoom.PrintRoom(enemies) ;
            string input = Console.ReadLine();
            if (input == "quit")
            {
                break;
            }

            if (input.StartsWith("pick up"))
            {
                // do something
                if (currentRoom.Items.Count > 0)
                {
                    for (int i = 0; i < currentRoom.Items.Count; i++)
                    {
                        if (input.Contains(currentRoom.Items[i].Name))
                        {
                            itemFlg = 1;
                            scoreboard.Score += currentRoom.Items[i].PointValue;
                            inventory.Add(currentRoom.Items[i]);
                            currentRoom.Items.RemoveAt(i);
                            validAction = true;
                            break;
                        }
                    }
                }

                if (itemFlg == 0)
                {
                    Console.WriteLine("You don't find that item in this room.");
                    // valid action -> true;
                    validAction = true;
                }

            }
            else if (input.StartsWith("drop"))
            {
                // do something
                if (inventory.Count > 0)
                {
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        if (input.Contains(inventory[i].Name))
                        {
                            itemFlg = 1;
                            scoreboard.Score -= inventory[i].PointValue;
                            currentRoom.Items.Add(inventory[i]);
                            inventory.RemoveAt(i);
                            validAction = true;
                            break;
                        }
                    }

                }

                if (itemFlg == 0)
                {
                    Console.WriteLine("You don't have that item.");
                    // valid action -> true;
                    validAction = true;
                }
            }
            else if (input.StartsWith("describe"))
            {
                if (inventory.Count > 0)
                {
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        if (input.ToLower().Contains(inventory[i].Name.ToLower()))
                        {
                            itemFlg = 1;
                            Console.WriteLine("The item is in your inventory.");
                            Console.WriteLine(inventory[i].Description);
                            validAction = true;
                            break;
                        }
                    }
                }
                if (currentRoom.Items.Count > 0 && itemFlg == 0)
                {
                    for (int i = 0; i < currentRoom.Items.Count; i++)
                    {
                        if (input.ToLower().Contains(currentRoom.Items[i].Name.ToLower()))
                        {
                            itemFlg = 1;
                            Console.WriteLine("The item is in the current room.");
                            Console.WriteLine(currentRoom.Items[i].Description);
                            validAction = true;
                            break;
                        }
                    }
                }
                if (itemFlg == 0)
                {
                    Console.WriteLine("you don't have that item");
                    // valid action -> true;
                    validAction = true;
                }
                continue;
            }

            // add attack action
            // if the user input is "attack 'attackable' with 'weapon'", break an enemy or an item
            else if (input.StartsWith("attack"))
            {
                // input check
                var regex = new Regex(@"attack ([\w\s]+) with ([\w\s]+)", RegexOptions.IgnoreCase);
                var match = regex.Match(input);
                // if it matches, check if the input is correct
                if (match.Success)
                {
                    // check the attackable item is in the current room
                    string attackableName = match.Groups[1].Value.ToLower();
                    string weaponName = match.Groups[2].Value;
                    // find matching weapon
                    IWeapon weapon = FindWeapon(weaponName);
                    // find matching enemy in room
                    IAttackable attackable = FindAttackable(attackableName);
                    if (weapon != null && attackable != null && !attackable.IsDestroyed)
                    {
                        int attackResult = weapon.Attack(attackable);
                        // attackResult 1: Enemy
                        //                 Put attackable.newItem in the currentRoom
                        if (attackResult == 1)
                        {
                            while (true)
                            {
                                Console.WriteLine($"Do you want to take {(attackable as Enemy).Name} into the circle?(Y/N) ");
                                String circleFlg = Console.ReadLine().ToLower();
                                if (circleFlg == "y")
                                {
                                    inventory.Add(attackable as Enemy);
                                    Console.WriteLine($"Congratulations. {(attackable as Enemy).Name} has joined your party!");
                                    for (int i=0; i < enemies.Count; i++)
                                    {
                                        if (enemies[i].Name.ToLower() == attackableName)
                                        {
                                            enemies.RemoveAt(i);
                                            break;
                                        }
                                    }
                                    break;
                                } else if (circleFlg == "n")
                                {
                                    break;
                                } else
                                {
                                    Console.WriteLine("Invalid input. Try again.");
                                    continue;
                                }
                            }
                        
                        } else if (attackResult == 2)
                        {
                            currentRoom.Items.Add(attackable.NewItem);
                        }
                        
                        validAction = true;
                    } else
                    {
                        Console.WriteLine($"You cannot attack {attackableName} with {weaponName}.");
                        validAction = true;
                    }
                    
                } else
                {
                    Console.WriteLine("You don't find that item, enemy, or weapon.");
                }
            }

            // Save function
            // when the user inputs "save", the program will save the data such as items, the user will be able to use the data if he/she types "load"
            else if (input.StartsWith("save"))
            {
                SaveGame();
                validAction = true;
            }

            // Load function
            // when the user inputs "load", the program will load the data that the user saved
            else if (input.StartsWith("load"))
            {
                finishFlg = LoadGame();

            }
            if (finishFlg)
            {
                break;
            }

            if (currentRoom.Transitions.TryGetValue(input, out Room nextRoom))
            {
                // if the user tries to escape from an enemy, the enemy attacks the user
                foreach (Enemy enemy in enemies)
                {
                    if (currentRoom == enemy.enemyCurrentRoom)
                    {
                        if (!enemy.IsDestroyed)
                        {
                            Console.WriteLine($"The {enemy.Name} attacks and slays you.");
                            scoreboard.Score = 0;
                            slayedFlg = true;
                            break;
                        } 
                        
                    }
                }

                if (slayedFlg)
                {
                    break;
                }
                currentRoom = nextRoom;

                if (moveCount > 0 && moveCount % 2 == 0)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        if (!enemy.IsDestroyed)
                        {
                            Random randomGenerator = new Random();

                            int howManyTransitions = enemy.enemyCurrentRoom.Transitions.Count;
                            List<Room> candidates = new List<Room>();
                            foreach (Room room in enemy.enemyCurrentRoom.Transitions.Values)
                            {
                                if (!room.FinalRoom)
                                {
                                    candidates.Add(room);
                                }

                            }

                            int randIndex = randomGenerator.Next(candidates.Count);
                            enemy.enemyCurrentRoom = candidates[randIndex];
                        }
                        
                        enemyMoveFlg = true;
                    }
                    
                }

                moveCount += 1;
                validAction = true;
            } 

            if (!validAction)
            {
                Console.WriteLine("Invalid action.");
            }

            if (currentRoom.FinalRoom)
            {
                currentRoom.PrintRoom();
                break;
            }

        }
    }

    private IAttackable FindAttackable(string attackableName)
    {
        Item item = currentRoom.Items.Find(item => item.Name.ToLower() == attackableName.ToLower());
        Enemy enemy = enemies.Find(enemy => enemy.Name.ToLower() == attackableName.ToLower() && enemy.enemyCurrentRoom.RoomName == currentRoom.RoomName);
        IAttackable attackableItem = item as IAttackable;
        IAttackable attackableEnemy = enemy as IAttackable;
        if (attackableItem == null && attackableEnemy == null)
        {
            Console.WriteLine($"There is no {attackableName} in this room.");
        }

        return attackableEnemy ?? attackableItem;
    }

    private IWeapon FindWeapon(string weaponName)
    {
        Item item = inventory.Find(item => item.Name.ToLower() == weaponName.ToLower());
        WeaponItem weapon1 = item as WeaponItem;
        if (weapon1 == null)
        {
            Console.WriteLine("You don't have ", weaponName, ".");
        }

        return weapon1;
    }

    public void SaveGame()
    {
        try
        {
            String fileName = "SavedGame.xml";
            XmlSerializer writer = new XmlSerializer(typeof(InheritorGame));
            StreamWriter stream = new StreamWriter(fileName);
            writer.Serialize(stream, this);
            stream.Close();
            Console.WriteLine("Save complete!");
        } catch(Exception)
        {
            throw;
        }
        
    }

    public bool LoadGame()
    {
        String fileName = "SavedGame.xml";
        XmlSerializer writer = new XmlSerializer(typeof(InheritorGame));
        StreamReader stream = new StreamReader(fileName);
        InheritorGame loadedGame = (InheritorGame)writer.Deserialize(stream);
        stream.Close();

        Console.WriteLine("Loaded!");

        //loadedGame.SetupRooms();
        loadedGame.InputLoop();
        return true;
    }

    public void PrintScore()
    {
        Console.WriteLine($"Your score was: {scoreboard.Score}");
    }



}