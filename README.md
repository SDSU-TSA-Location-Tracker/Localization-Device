# Localization-Device
This pocket embedded device acts as a local node in a network of anchor nodes and a gateway node. The local device transmits data to the anchor nodes, which measure the RSSI between multiple anchor nodes and the local node to estimate the distance between anchors in the network; this estimation is done through a linear regression machine learning model. The transmission and receiving of data between local and anchor nodes will be performed using BLE technology. The distance estimation is done at the gateway node, where the RSSI values are sent. One layer up, a holistic system calculates the position of the local node based on the distance from each anchor node. Our design utilizes embedded design with microcontrollers, power and networking components, and networking and machine learning algorithms. 

## Project Team Members
Leonard Chatterton (CompE), Hada Marabeh (EE), Tristan Richmond (CompE), Sri Shatagopam (CompE), Zachary Rocha (EE)
 
## How to Contribute to the Repository
Here is a brief explanation on how to contribute git and github

#### Git vs. Github
Git is a repository version control that is local to your computer only. This means only the user of the local computer has access to these files. It keeps track of any changes that are commited to it so that each iteration of a repo can be accessed. Be sure to update this each time you finish your work session for the day as it will act as a backup storage. Git is important for version control but also because this is where github will be pulling the files from whenever a "push" occurs. **You will need to install git onto your computer**

https://git-scm.com/downloads

Github is a cloud based repository viewer that holds allows a repository to be accessed via webapp. Github will act as a central location for all our individual tasks and develepments to be accessed from any computer connected to the internet. Changes should be comitted to Git first and then it is pushed to Github so anybody can access the actual files. 

#### Cloning
To make the initial link of a github repository onto your git, you must clone it. Here are the steps to doing so

1. Open git bash or linux terminal in the working directory of where you wish to store the repository files on your computer. This is can be done by right-clicking inside the folder you save the files and selecting "Git Bash Here"

2. Use the clone command to clone the github repository onto your git. It will automatically create a folder with the name of the repository with all of its contents
```
git clone https://github.com/SDSU-TSA-Location-Tracker/Localization-Device.git
```

#### Branching
Every repository, in our case **Localization-Device**, has a **main** branch where the most up-to-date ready to ship version is held. This is to ensure there is always a deployable version at any point in the development and maintenance cycle. When new features or additions are being developed, typically a new branch is created with the name of that feature. This allows for a new location that new code and updates can be saved without affecting the **main** branch. In our case, there currently exists a branch for each seperate piece to the project. Once one of the features/branches has been thoroughly tested and is stable, that branch is merged into the **main** branch. What this will do is copy all files from the initial development branch and merge them into the **main** branch so it becomes the most up-to-date version with these new features.

Here is an example of how to create a new branch off of main. Please use the following naming format for the branch **[Your_Name]_[Description of Branch]**

#### Github Branch Example

Create a new branch off of **RaspberryPi** called "Tristan_RaspberryPi_PCB_Design"
```
git checkout -b Tristan_RaspberryPi_PCB_Design RaspberryPi
```
#### Git Workflow

Here is a step by step explanation of how a typical worksession goes

1. Open git bash or linux terminal in the working directory of where you store the repository files on your computer. This is can be done by right-clicking inside the folder you save the files and selecting "Git Bash Here"

2. Pull any and all changes from the github repository **main** to be saved into your local git. Note you must enter the exact branches you wish to update your local version of to the github version. See step 3 on how to change current branch. 
```
git pull origin [Name_of_Branch]
```

3. Enter the exact branch you wish to do development in. Important to do this step as failure to do so may result in conflicts
```
git checkout [Name_of_Branch]
```

4. Start developing as you normally do until you are all finished

5. Add the specific files that were worked
```
git add file.txt
```

6. Commit the changes to the added files to the local git with a message of what was changed. It is important to add a message as the commit will open up VIM expecting you to write a message in. **To avoid this issue, be sure to write the message in the initial commit command**

```
git commit -m "Programmed the Arduino to blink an LED every 5 seconds"
```

7. Push these commits in your local git to the github so everyone can pull these changes to their local computers
```
git push origin [Name_of_Branch]
```

8. A pop window will appear prompting for you github login information. Enter you username into the first window and click submit, then enter you password into the second window and click submit.
