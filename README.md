<!-- Written by Mohammad Ishrak Abedin-->
# \[PICS\] Parametric Image Collection System
---
### A modular image collection application based on JSON parameters for image data collection
---

## *Framework*
---
The application is written in [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) and built using [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/).

## *Libraries and  Dependencies*
---
+ [WebEye.Controls.Wpf.WebCameraControl](https://www.nuget.org/packages/WebEye.Controls.Wpf.WebCameraControl/)
+ [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/)
+ [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)

## *Writing Custom Experiment Parameters*
---
All experiment data are drawn from [ExperimentData.json](./PICS/ExperimentData.json) file, located in the same directory as the executable. 3 Types of values should be present there.

```json
{
    "IterationCount": 5,
    "SaveDir": "./Data/",
    "ExperimentList": [
        {
            "Experiment1 Key1": "Experiment1 Value1",
            "Experiment1 Key2": "Experiment1 Value2",
            "Save Tag": "Experiment1 Tag"
        },
        {
            "Experiment2 Key1": "Experiment2 Value1",
            "Experiment2 Key2": "Experiment2 Value2",
            "Save Tag": "Experiment2 Tag"
        }
    ]
}
```
+ `IterationCount` key denotes how many time each single experiment must be repeated. Make sure the value is at least 1.
+ `SaveDir` is the base directory under which collected data will be saved. New folders will be created as per requirement based on the user and camera information. 
+ `ExperimentList` is a list of Key Value pairs (KVPs). Each single KVP represents a single experiment that would be repeated `IterationCount` amount of times. Each KVP much have `Save Tag`, since it will be used as a name to save collected images. A number based on iteration will be appended after it, separated by an underscore ('_'). Images are saved in PNG format.

## *Using the Application*
---
1. Before starting any experiment, fill in user detail first located on top right.
2. Then select a valid camera from the drop down list.
3. Start the camera and use the capture button to take images. the capture button shows a badge denoting the iteration count for the current experiment.
4. Experiments can be cycled left and right using the arrow buttons. Any experiment for which all iterations are completed will have a check mark on top.
5. Keyboard shortcuts can be used to perform image taking tasks. Press `F1` on keyboard to bring up help that will show other keyboard shortcuts.