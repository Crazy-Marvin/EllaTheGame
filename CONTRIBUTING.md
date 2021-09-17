# Localization

We use a JSON file located in [/docs/jsonFile.json](https://crazy-marvin.github.io/EllaTheGame/jsonFile.json) for localization which gets served using [GitHub Pages](https://crazy-marvin.github.io/EllaTheGame/jsonFile.json).

# Adding Translations
Translations are represented by a Json Key-Value combination
**Example**
    {
        "en": value
    }
## "Value"
 - The value here is an array of words
 - You should keep the words in the Array in the same order as the original english Translation 
**Like This**
    ["en","Choose a Level","Difficulty","Puppy","Adult","Senior","Exit","Score","Share It","Replay","Main Menu","Resume"]
**Important**
 The first element in the array is the language name. Here I Used a 2 words language code, but you can use the complete name of the language or any other code format.
 **Example A**
 ["en","Choose a Level","Difficulty","Puppy","Adult","Senior","Exit","Score","Share It","Replay","Main Menu","Resume"]
 **Example B**
  ["English","Choose a Level","Difficulty","Puppy","Adult","Senior","Exit","Score","Share It","Replay","Main Menu","Resume"].

As you can see in Example A & B, we used "English" instead of "en", and both of them are ok.
## Note
You should keep the same order of the translations

**Example**
["English","Choose a Level","Difficulty","Puppy","Adult","Senior","Exit","Score","Share It","Replay","Main Menu","Resume"].
--------------------------------
["Français","Choisir un niveau","Difficulté","Chiot","Adulte","Senior","Sortir","Score","Partager","Replay","Menu principal"," Reprendre"].