using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

// Example Yahtzee website if you've never played
// https://cardgames.io/yahtzee/

namespace Yahtzee
{
    public partial class yahtzeeForm : Form
    {
        public yahtzeeForm()
        {
            InitializeComponent();
        }

        // you may find these helpful in manipulating the scorecard and in other places in your code
        private const int NONE = -1;
        private const int ONES = 0;
        private const int TWOS = 1;
        private const int THREES = 2;
        private const int FOURS = 3;
        private const int FIVES = 4;
        private const int SIXES = 5;
        private const int THREE_OF_A_KIND = 6;
        private const int FOUR_OF_A_KIND = 7;
        private const int FULL_HOUSE = 8;
        private const int SMALL_STRAIGHT = 9;
        private const int LARGE_STRAIGHT = 10;
        private const int CHANCE = 11;
        private const int YAHTZEE = 12;

        private int rollCount = 0;
        private int uScoreCardCount = 0;

        private bool userTurn = true;

        private int computerRollCount = 0;
        private int cScoreCardCount = 0;

        // you'll need an instance variable for the user's scorecard - an array of 13 ints
        private int[] userScorecard = new int[13];
        private int[] computerScorecard = new int[13];

        // as well as an instance variable for 0 to 5 dice as the user rolls - array or list<int>?
        private List<int> roll = new List<int>();

        // as well as an instance variable for 0 to 5 dice that the user wants to keep - array or list<int>?
        private List<int> keep = new List<int>();

        // this is the list of methods that I used

        // START WITH THESE 2
        // This method rolls numDie and puts the results in the list
        public void Roll(int numDie, List<int> roll)
        {
            //create random number generator
            Random randomNumberGenerator = new Random();
            
            //clear the roll list and set capacity to 5
            roll.Clear();
            roll.Capacity = 5;
            
            //loop through the roll list and set every element to -1 (no element)
            for(int i = 0; i < 5; i++)
            {
                roll.Add(-1);
            }

            //for every die that needs rolled
            for (int i = 0; i < numDie; i++)
            {
                int randomPosition;

                //set a random position for the picture box (0-4)
                do
                {
                    randomPosition = randomNumberGenerator.Next(0, 5);
                } while (roll[randomPosition] != -1); //if the position is already filled, pick a new random position

                //after finding an empty position, roll the die and put it in that position
                roll[randomPosition] = randomNumberGenerator.Next(1, 7);
                //roll.Add(i + 1);
                //roll.Add(5);
                //if (i < 3)
                //    roll.Add(5);
                //else
                //    roll.Add(3);
            }
        }

        // This method moves all of the rolled dice to the keep dice before scoring.  All of the dice that
        // are being scored have to be in the same list 
        public void MoveRollDiceToKeep(List<int> roll, List<int> keep)
        {
            int rolledDice = roll.Count;
            for(int i = 0; i < rolledDice; i++)
            {
                keep.Add(roll[0]);
                roll.RemoveAt(0);
            }
        }

        #region Scoring Methods
        /* This method returns the number of times the parameter value occurs in the list of dice.
         * Calling it with 5 and the following dice 2, 3, 5, 5, 6 returns 2.
         */
        private int Count(int value, List<int> dice)
        {
            int count = 0;
            foreach (int dieValue in dice)
            {
                if (value == dieValue)
                    count++;
            }
            //take keep list, use contains??
            return count;
        }

        /* This method counts how many 1s, 2s, 3s ... 6s there are in a list of ints that represent a set of dice
         * It takes a list of ints as it's parameter.  It should create an array of 6 integers to store the counts.
         * It should then call Count with a value of 1 and store the result in the first element of the array.
         * It should then repeat the process of calling Count with 2 - 6.
         * It returns the array of counts.
         * All of the rest of the scoring methods can be "easily" calculated using the array of counts.
         */
        private int[] GetCounts(List<int> dice)
        {
            int[] counts = new int[6];
            for(int i = ONES; i <= SIXES; i++)
            {
                int value = i + 1;
                counts[i] = Count(value, dice);
            }
            return counts;
        }

        /* Each of these methods takes the array of counts as a parameter and returns the score for a dice value.
         */
        private int ScoreOnes(int[] counts)
        {
            int score = counts[ONES] * 1;
            if (userTurn)
            {
                userScorecard[ONES] = score;
            }
            else
            {
                computerScorecard[ONES] = score;
            }
            return score;
        }

        private int ScoreTwos(int[] counts)
        {
            int score = counts[TWOS] * 2;
            if (userTurn)
            {
                userScorecard[TWOS] = score;
            }
            else
            {
                computerScorecard[TWOS] = score;
            }
            return score;
        }

        private int ScoreThrees(int[] counts)
        {
            int score = counts[THREES] * 3;
            if (userTurn)
            {
                userScorecard[THREES] = score;
            }
            else
            {
                computerScorecard[THREES] = score;
            }
            return score;
        }

        private int ScoreFours(int[] counts)
        {
            int score = counts[FOURS] * 4;
            if (userTurn)
            {
                userScorecard[FOURS] = score;
            }
            else
            {
                computerScorecard[FOURS] = score;
            }
            return score;
        }

        private int ScoreFives(int[] counts)
        {
            int score = counts[FIVES] * 5;
            if (userTurn)
            {
                userScorecard[FIVES] = score;
            }
            else
            {
                computerScorecard[FIVES] = score;
            }
            return score;
        }

        private int ScoreSixes(int[] counts)
        {
            int score = counts[SIXES] * 6;
            if (userTurn)
            {
                userScorecard[SIXES] = score;
            }
            else
            {
                computerScorecard[SIXES] = score;
            }
            return score;
        }

        /* This method can be used to determine if you have 3 of a kind (or 4? or  5?).  The output parameter
         * whichValue tells you which dice value is 3 of a kind.
         */
        private bool HasCount(int howMany, int[] counts, out int whichValue)
        {
            int index = ONES;
            foreach (int count in counts)
            {
                if (howMany <= count)
                {
                    whichValue = index;
                    return true;
                }
                index++;
            }
            whichValue = NONE;
            return false;
        }

        /* This method returns the sum of the dice represented in the counts array.
         * The sum is the # of 1s * 1 + the # of 2s * 2 + the number of 3s * 3 etc
         */ 
        private int Sum(int[] counts)
        {
            int sum = 0;
            for (int i = 0; i < counts.Length; i++) {
                sum += (counts[i] * (i + 1));
            }
            return sum;
        }

        /* This method calls HasCount(3...) and if there are 3 of a kind calls Sum to calculate the score.
         */ 
        private int ScoreThreeOfAKind(int[] counts)
        {
            int value;
            if (HasCount(3, counts, out value))
            {
                if (userTurn)
                    userScorecard[THREE_OF_A_KIND] = Sum(counts);
                else
                    computerScorecard[THREE_OF_A_KIND] = Sum(counts);
                return Sum(counts);
            }
            return 0;
        }

        private int ScoreFourOfAKind(int[] counts)
        {
            int value;
            if (HasCount(4, counts, out value))
            {
                if (userTurn)
                    userScorecard[FOUR_OF_A_KIND] = Sum(counts);
                else
                    computerScorecard[FOUR_OF_A_KIND] = Sum(counts);
                return Sum(counts);
            }
            return 0;
        }

        private int ScoreYahtzee(int[] counts)
        {
            int value;
            if (HasCount(5, counts, out value)) {
                if (userTurn)
                    userScorecard[YAHTZEE] = 50;
                else
                    computerScorecard[YAHTZEE] = 50;
                return 50;
            }
            return 0;
        }

        /* This method calls HasCount(2 and HasCount(3 to determine if there's a full house.  It calls sum to 
         * calculate the score.
         */ 
        private int ScoreFullHouse(int[] counts)
        {
            int value1;
            int value2;
            if (HasCount(2, counts, out value1) && HasCount(3, counts, out value2))
            {
                if (userTurn)
                    userScorecard[FULL_HOUSE] = 25;
                else
                    computerScorecard[FULL_HOUSE] = 25;
                return 25;
            }
            return 0;
        }

        private int ScoreSmallStraight(int[] counts)
        {
            //check for all three small straights (1234 or 2345 or 3456)
            for (int i = 0; i < 3; i++)
            {
                if (counts[i] != 0 && counts[i + 1] != 0 && counts[i + 2] != 0 && counts[i + 3] != 0)
                {
                    //if there is a small straight, score is 30
                    if (userTurn)
                        userScorecard[SMALL_STRAIGHT] = 30;
                    else
                        computerScorecard[SMALL_STRAIGHT] = 30;
                    return 30;
                }
            }
            //if there is no small straight, score is 0
            return 0;
        }

        private int ScoreLargeStraight(int[] counts)
        {
            //check for both large straights (12345 or 23456)
            for (int i = 0; i < 2; i++)
            {
                if (counts[i] != 0 && counts[i + 1] != 0 && counts[i + 2] != 0 && counts[i + 3] != 0 && counts[i + 4] != 0)
                {
                    //if there is a large straight, score is 40
                    if (userTurn)
                        userScorecard[LARGE_STRAIGHT] = 40;
                    else
                        computerScorecard[LARGE_STRAIGHT] = 40;
                    return 40;
                }
            }
            //if there is no large straight, score is 0
            return 0;
        }

        private int ScoreChance(int[] counts)
        {
            if (userTurn)
                userScorecard[CHANCE] = Sum(counts);
            else
                computerScorecard[CHANCE] = Sum(counts);
            return Sum(counts);
        }

        /* This method makes it "easy" to call the "right" scoring method when you click on an element
         * in the user score card on the UI.
         */ 
        private int Score(int whichElement, List<int> dice)
        {
            int[] counts = GetCounts(dice);
            switch (whichElement)
            {
                case ONES:
                    return ScoreOnes(counts);
                case TWOS:
                    return ScoreTwos(counts);
                case THREES:
                    return ScoreThrees(counts);
                case FOURS:
                    return ScoreFours(counts);
                case FIVES:
                    return ScoreFives(counts);
                case SIXES:
                    return ScoreSixes(counts);
                case THREE_OF_A_KIND:
                    return ScoreThreeOfAKind(counts);
                case FOUR_OF_A_KIND:
                    return ScoreFourOfAKind(counts);
                case FULL_HOUSE:
                    return ScoreFullHouse(counts);
                case SMALL_STRAIGHT:
                    return ScoreSmallStraight(counts);
                case LARGE_STRAIGHT:
                    return ScoreLargeStraight(counts);
                case CHANCE:
                    return ScoreChance(counts);
                case YAHTZEE:
                    return ScoreYahtzee(counts);
                default:
                    return 0;
            }
        }
        #endregion

        // set each value to some negative number because 
        // a 0 or a positive number could be an actual score
        private void ResetScoreCard(int[] scoreCard, int scoreCardCount)
        {
            //itterate through the scorecard and fill each spot with -1 (no value)
            for (int i = 0; i < scoreCard.Length; i++)
            {
                scoreCard[i] = -1;
                computerScorecard[i] = -1;
            }

            scoreCardCount = 0;
            cScoreCardCount = 0;

            //populate the keep list with -1 (no value)
            for (int i = 0; i < 5; i++)
            {
                keep.Add(-1);
            }
        }

        // this set has to do with user's scorecard UI
        private void ResetUserUIScoreCard()
        {
            //itterate through user0 - user12 and reset them
            for (int i = 0; i < 13; i++)
            {
                Label scoreCardElement = (Label)this.scoreCardPanel.Controls["user" + i];
                scoreCardElement.Text = "";
                scoreCardElement.Enabled = true;
            }

            //reset userSum, userTotalScore, and userBonus
            userSum.Text = "";
            userSum.Enabled = true;
            userTotalScore.Text = "";
            userTotalScore.Enabled = true;
            userBonus.Text = "";
            userBonus.Enabled = true;
        }

        // this method adds the subtotals as well as the bonus points when the user is done playing
        public void UpdateUserUIScoreCard()
        {
            int totalScore = 0;

            //add up the top scores (ONES - SIXES)
            for (int i = 0; i < 6; i++)
            {
                totalScore += userScorecard[i];
            }

            //if the total top score is >= 63, give a bonus of 35
            if (totalScore >= 63)
            {
                userBonus.Text = "35";
                totalScore += 35;
            }
            else
            {
                //otherwise the bonus is 0
                userBonus.Text = "0";
            }

            //update the sum text
            userSum.Text = totalScore.ToString();

            //add all the bottom scores to totalScore
            for (int i = 6; i < 13; i++)
            {
                totalScore += userScorecard[i];
            }

            userTotalScore.Text = totalScore.ToString();
        }

        /* When I move a die from roll to keep, I put a -1 in the spot that the die used to be in.
         * This method gets rid of all of those -1s in the list.
         */
        private void CollapseDice(List<int> dice)
        {
            int numDice = dice.Count;
            for (int count = 0, i = 0; count < numDice; count++)
            {
                if (dice[i] == -1)
                    dice.RemoveAt(i);
                else
                    i++;
            }
        }

        /* When I move a die from roll to keep, I need to know which pb I can use.  It's the first spot with a -1 in it
         */
        public int GetFirstAvailablePB(List<int> dice)
        {
            return dice.IndexOf(-1);
        }

        #region UI Dice Methods
        /* These are all UI methods */
        private PictureBox GetKeepDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["keep" + i];
            return die;
        }

        public void HideKeepDie(int i)
        {
            GetKeepDie(i).Visible = false;
        }

        public void HideAllKeepDice()
        {
            for (int i = 0; i < 5; i++)
                HideKeepDie(i);
        }

        public void ShowKeepDie(int i)
        {
            PictureBox die = GetKeepDie(i);
            die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + keep[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllKeepDie()
        {
            for (int i = 0; i < 5; i++)
            {
                ShowKeepDie(i);
            }
        }

        private PictureBox GetComputerKeepDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["computerKeep" + i];
            return die;
        }

        public void HideComputerKeepDie(int i)
        {
            GetComputerKeepDie(i).Visible = false;
        }

        public void HideAllComputerKeepDice()
        {
            for (int i = 0; i < 5; i++)
                HideComputerKeepDie(i);
        }

        public void ShowComputerKeepDie(int i)
        {
            PictureBox die = GetComputerKeepDie(i);
            die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + keep[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllComputerKeepDie()
        {
            for (int i = 0; i < 5; i++)
            {
                ShowComputerKeepDie(i);
            }
        }

        private PictureBox GetRollDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["roll" + i];
            return die;
        }

        public void HideRollDie(int i)
        {
            GetRollDie(i).Visible = false;
        }

        public void HideAllRollDice()
        {
            for (int i = 0; i < 5; i++)
                HideRollDie(i);
        }

        public void ShowRollDie(int i)
        {
            PictureBox die = GetRollDie(i);
            die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + roll[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllRollDie()
        { 
            for (int i = 0; i < 5; i++)
            {
                if (roll[i] == -1)
                    continue;
                ShowRollDie(i);
            }
        }
        #endregion

        #region Event Handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            ResetScoreCard(userScorecard, uScoreCardCount);
            ResetUserUIScoreCard();
            HideAllRollDice();
            HideAllKeepDice();
            HideAllComputerKeepDice();

             /* reset the user's scorecard
             * Hide the roll dice
             * Hide the keep dice
             * Hide the computer's dice
             */ 
        }

        private void rollButton_Click(object sender, EventArgs e)
        {
            // DON'T WORRY ABOUT ROLLING MULTIPLE TIMES UNTIL YOU CAN SCORE ONE ROLL
            // hide all of the keep picture boxes
            // any of the die that were moved back and forth from roll to keep by the user
            // are "collapsed" in the keep data structure
            // show the keep dice again

            // START HERE
            // clear the roll data structure
            //roll.Clear();
            
            // hide all of the roll picture boxes
            HideAllRollDice();

            // roll the right number of dice
            int numDiceToRoll = 5;
            foreach(int value in keep)
            {
                if (value != -1)
                    numDiceToRoll--;
            }
            Roll(numDiceToRoll, roll);

            // show the roll picture boxes
            ShowAllRollDie();

            // increment the number of rolls
            rollCount++;

            // disable the button if you've rolled 3 times
            if (rollCount >= 3)
                rollButton.Enabled = false;
        }

        private void userScorecard_DoubleClick(object sender, EventArgs e)
        {
            // move any rolled die into the keep dice
            MoveRollDiceToKeep(roll, keep);
            
            // hide picture boxes for both roll and keep
            HideAllKeepDice();
            HideAllRollDice();
            
            // determine which element in the score card was clicked
            Label clickedElement = (Label)sender;
            int elementNumber = int.Parse(clickedElement.Name.Substring(4));
            
            // score that element
            int score = Score(elementNumber, keep);
            
            // put the score in the scorecard and the UI
            userScorecard[elementNumber] = score;
            clickedElement.Text = score.ToString();
            
            // disable this element in the score card
            clickedElement.Enabled = false;

            // clear the keep dice
            keep.Clear();
            for (int i = 0; i < 5; i++)
            {
                keep.Add(-1);
            }

            // reset the roll count
            rollCount = 0;

            // increment the number of elements in the score card that are full
            uScoreCardCount++;

            // enable/disable buttons
            rollButton.Enabled = true;

            // when it's the end of the game
            if(uScoreCardCount == 13)
            {
                UpdateUserUIScoreCard();
                rollButton.Enabled = false;
            }

            //userTurn = false;
            //ComputerPlay();
            // update the sum(s) and bonus parts of the score card
            // enable/disable buttons
            // display a message box?
        }

        private void roll_DoubleClick(object sender, EventArgs e)
        {
            // figure out which die you clicked on
            PictureBox die = (PictureBox)sender;
            int clickedDie = int.Parse(die.Name.Substring(4, 1));

            // figure out where in the set of keep picture boxes there's a "space"
            int availableSpot = GetFirstAvailablePB(keep);

            // move the roll die value from this die to the keep data structure in the "right place"
            keep[availableSpot] = roll[clickedDie];


            // sometimes that will be at the end but if the user is moving dice back and forth
            // it may be in the middle somewhere

            // clear the die in the roll data structure
            keep[availableSpot] = roll[clickedDie];
            roll[clickedDie] = -1;
            ShowKeepDie(availableSpot);
            HideRollDie(clickedDie);
            // hide the picture box
        }

        private void keep_DoubleClick(object sender, EventArgs e)
        {
            // figure out which die you clicked on
            PictureBox die = (PictureBox)sender;
            int clickedDie = int.Parse(die.Name.Substring(4, 1));

            // figure out where in the set of roll picture boxes there's a "space"
            int availableSpot = GetFirstAvailablePB(roll);

            // move the roll die value from this die to the roll data structure in the "right place"
            // sometimes that will be at the end but if the user is moving dice back and forth
            // it may be in the middle somewhere
            roll[availableSpot] = keep[clickedDie];
            keep[clickedDie] = -1;
            ShowRollDie(availableSpot);
            HideKeepDie(clickedDie);


            // clear the die in the keep data structure
            // hide the picture box
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            ResetScoreCard(userScorecard, uScoreCardCount);
            ResetUserUIScoreCard();
            HideAllRollDice();
            HideAllKeepDice();
            HideAllComputerKeepDice();
            rollCount = 0;
            rollButton.Enabled = true;
        }
        #endregion

        #region Computer AI
        private void ComputerPlay()
        {
            ComputerRoll();

            Thread.Sleep(2000);

            ComputerRoll();

            Thread.Sleep(2000);

            ComputerRoll();

            Thread.Sleep(2000);

            //while (computerRollCount < 4)
            //{
            //    ComputerRoll();

            //    //add time here
            //    Thread.Sleep(1000);

            //    if (computerRollCount == 3)
            //    {
            //        ComputerScore();
            //        userTurn = true;

            //        //increase computer roll count to break the while loop
            //        computerRollCount++;
            //    }
            //    else if (computerRollCount < 3)
            //    {
            //        ComputerSelectDiceToKeep();
            //    }
            //}

        }

        private void ComputerRoll()
        {
            //HideAllRollDice();

            // roll the right number of dice
            int numDiceToRoll = 5;
            foreach (int value in keep)
            {
                if (value != -1)
                    numDiceToRoll--;
            }
            Roll(numDiceToRoll, roll);

            // show the roll picture boxes
            ShowAllRollDie();

            // increment the number of rolls
            computerRollCount++;
        }

        private void ComputerSelectDiceToKeep()
        {

        }

        private void ComputerScore()
        {

        }



        #endregion
    }
}
