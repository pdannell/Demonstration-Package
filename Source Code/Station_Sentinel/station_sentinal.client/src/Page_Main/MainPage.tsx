/*import { useEffect, useState } from 'react';*/
import './MainPage.css';
import Chart from 'chart.js/auto'
import { clone } from 'chart.js/helpers';
import { /*EventHandler,*/ useState } from 'react';

function App() { 

    //Set our page handler for displayed content.
    const [pageState, setState] = useState(0);

    //States for Support Page
    const [firstName, setFirstName] = useState("")
    const [lastName, setLastName] = useState("")
    const [emailAddress, setEmailAddress] = useState("")
    const [textBody, setTextBody] = useState("")

    //States For Log Search Page
    const [searchFor, setSearchFor] = useState("")
    const [startSearchDate, setStartSearchDate] = useState("")
    const [endSearchDate, setEndSearchDate] = useState("")

    //States For FileExplorer Page
    const [currentFolder, setCurrentFolder] = useState("")
    const [forwardFolder, setForwardFolder] = useState("")
    const [initializedFolder, setInitFolder] = useState("")

    //Determine which page to render based on userclicks.
    //Redrawing is fired when useState is updated.
    switch (pageState) {
        case 1: return (Click_CycleInformation());
        case 2: return (Click_FaultInformation());
        case 3: return (Click_Graphs());
        case 4: return (Click_LogSearch());
        case 5: return (Click_FileBrowser());
        case 6: return (Click_StationInfo());   //Placeholder For Future Use
        case 7: return (Page_Main());   //Placeholder For Future Use
        case 8: return (Page_Main());   //Placeholder For Future Use
        case 9: return (Click_Support());
        default: return (Page_Main());
    }

    //Function which handles page delegation when GUI clicks occur.
    function Handler_ClickMenus(val: number) {
        setState(val);
    }

    // #region ---Pages and Handlers Per Page
    // #region -Start Page 
    function Page_Main() {

        return (
                <div className="container">
                {/*---Header Start*/}
                <thead>
                    <h1>Station Sentinel</h1>
                </thead>
                <div></div>

                {/*---Button Grid Start*/}
                <div className="grid-container">
 
                    <button onClick={() => Handler_ClickMenus(1)}>Cycle Information
                        <img className="btnImage" src="\src\Page_Main\Images\Repeat.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(2)}>Error Information
                        <img className="btnImage" src="\src\Page_Main\Images\PoliceCar.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(3)}>Graphs And Charts
                        <img className="btnImage" src="\src\Page_Main\Images\StocksDown.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(4)}>Log Search
                        <img className="btnImage" src="\src\Page_Main\Images\Logs.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(5)}>File Browser
                        <img className="btnImage" src="\src\Page_Main\Images\FileBrowse.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(6)}>Station Information
                        <img className="btnImage" src="\src\Page_Main\Images\Spy.png"></img>
                    </button>

                    <button onClick={() => Handler_ClickMenus(7)}>
                    </button>

                    <button onClick={() => Handler_ClickMenus(8)}>{/*Status Dashboard*/}
                        {/*<img className="btnImage" src="\src\Page_Main\Images\Status.png"></img>*/}
                    </button>

                    <button onClick={() => Handler_ClickMenus(9)}>Contact Support
                        <img className="btnImage" src="\src\Page_Main\Images\Support.png"></img>
                    </button>
                </div>
            </div>
        );
    }
    // #endregion

    // #region -Status Dashboard - Unused, per customer request.
    //function Click_StatusDashboard() {
    //    return (<div></div>)
    //}
    // #endregion

    // #region -Cycle Information
    function Click_CycleInformation() {

        GenerateInitialData()

        return (
            <div className="container">
                <thead><h1>The Time Keeper</h1></thead>
                <form>
                    <fieldset>
                        <legend>Select A TimeFrame</legend>
                        <div>
                            <input type="checkbox" id="1DayBox" onChange={() => GenerateLineGraph("CB1")}></input> <label className="Interval">1 Day</label>
                            <input type="checkbox" id="3DayBox" onChange={() => GenerateLineGraph("CB3")}></input> <label className="Interval">3 Days</label>
                            <input type="checkbox" id="5DayBox" onChange={() => GenerateLineGraph("CB5")}></input> <label className="Interval">5 Days</label>     
                            <input type="checkbox" id="7DayBox" onChange={() => GenerateLineGraph("CB7")}></input> <label className="Interval">7 Days</label>      
                         </div>
                     </fieldset>
                  
                    <canvas id="lineP"></canvas>

                    <br></br> <br></br>
                    <label>Choose An Timer For Displaying:</label>
                    <select name="timerDisplay" id="timerDisplay" onChange={() => GenerateBarGraph((document.getElementById("timerDisplay") as HTMLInputElement).value)}>
                        <option value="PlaceHolder1">Loading... Please Wait...</option>
                    </select>
                    <br></br>

                    <canvas id="bGraph"></canvas>

                    <div className="div-1" id="resultsBox">
                    </div>
                    <br></br>
                    <input type="submit" value="Go Back"></input>
                </form>
            </div>)
        }

    async function GenerateInitialData()
        {
            //Tell the server to cache initial data then execute the 1-Day stuff.
            const requestStringLine = "Cycle?taskType=Initial";
            await (await fetch(requestStringLine)).json();
            GenerateLineGraph("CB1");
    }

    async function GenerateLineGraph(cbCaller : string) {

        ////Grab all our checkboxes.
        const cb1 = document.getElementById("1DayBox") as HTMLInputElement;
        const cb3 = document.getElementById("3DayBox") as HTMLInputElement;
        const cb5 = document.getElementById("5DayBox") as HTMLInputElement;
        const cb7 = document.getElementById("7DayBox") as HTMLInputElement;

        //Line graph grabber for later.
        const lineGraphElement = document.getElementById("lineP");

        //Define our Start/End variable and define the startime for use..
        const startTime = new Date().toLocaleDateString();
        let endTime

        //Toggle all other checkboxes off when the user clicks any of them and set our time variables.
        if (cbCaller == "CB1") {
            cb1.checked = true;
            cb3.checked = false;
            cb5.checked = false;
            cb7.checked = false;

            //Set our entime to current-7-1
            const subtractTime = new Date(new Date().getTime() - (1 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else if (cbCaller == "CB3") {
            cb1.checked = false;
            cb3.checked = true;
            cb5.checked = false;
            cb7.checked = false;

            //Set our entime to current-3
            const subtractTime = new Date(new Date().getTime() - (3 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else if (cbCaller == "CB5") {
            cb1.checked = false;
            cb3.checked = false;
            cb5.checked = true;
            cb7.checked = false;

            //Set our entime to current-5
            const subtractTime = new Date(new Date().getTime() - (5 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else {
            cb1.checked = false;
            cb3.checked = false;
            cb5.checked = false;
            cb7.checked = true;

            //Set our entime to current-7
            const subtractTime = new Date(new Date().getTime() - (7 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        };

        //Request the data based on our users selections.
        let requestStringLine = "Cycle?taskType=CycleTimes&startTime=" + startTime + "&endtime=" + endTime;
        let responseDataLine: string[] = await (await fetch(requestStringLine)).json();

        //Formats values for our for graphing.
        const lineGraphLabels: number[] = [];
        const lineGraphValues: number[] = [];
        const lineBackgroundColor: string[] = [];
        const lineBordercolor: string[] = [];

        //Counter for which line we're on of our forEach
        let i: number = 1;

        //Loop through our value array and populate all our graphing data.
        responseDataLine.forEach((value) => {

            //Increment counter then populate our BCMP time and count.
            i += 1;
            lineGraphValues.push(parseInt(value.split('|')[0]));
            lineGraphLabels.push(i);

            //Populate if the point was a Pass or Fail
            if (value.split('|')[1] == "P") {
                lineBackgroundColor.push("green");
                lineBordercolor.push("green");
            }
            else {
                lineBackgroundColor.push("red");
                lineBordercolor.push("red");
            }
        });

        //Raw Graphing Data.
        const refinedLineData = {
            labels: lineGraphLabels,
            datasets: [{
                label: "BCMP To BCMP Timeline",
                data: lineGraphValues,
                pointBackgroundColor: lineBackgroundColor,
                pointBordercolor: lineBordercolor,
                pointRadius: 2.5,
                tension: 0.1
            }]
        };

        //Delete the old chart if it exists.
        const chart = Chart.getChart("lineP");
        chart?.destroy();

        new Chart(
            // @ts-expect-error "Library issue that doesn't seem to be resolveable."
            lineGraphElement,
            {
                type: 'scatter',
                data: refinedLineData,
                options: {
                    scales: {
                        y: {
                            max: 250,
                        }
                    }
                }
            }
        );

        //Populate the timer list for the time period.
        //Request the data based on our users selections.
        requestStringLine = "Cycle?taskType=TimerNames";
        responseDataLine = await (await fetch(requestStringLine)).json();

        //Fill our selection box the user.
        const selectionBox = document.getElementById("timerDisplay");
        //Clear Old Options
        while (selectionBox?.lastChild) {
            selectionBox?.removeChild(selectionBox.lastChild);
        }

        //Add a single empty selection so the user doesn't think they can just run stuff all willy nilly.
        const elementBlank = document.createElement("option");
        elementBlank.textContent = "";
        elementBlank.id = "TempSelections";
        selectionBox?.appendChild(elementBlank);

        //Add New Options
        responseDataLine.forEach((value) => {
            const element = document.createElement("option");
            element.textContent = value;
            element.id = "TempSelections";
            selectionBox?.appendChild(element);
        })
    }

    async function GenerateBarGraph(searchText: string) {

        ////Grab all our checkboxes.
        const cb1 = document.getElementById("1DayBox") as HTMLInputElement;
        const cb3 = document.getElementById("3DayBox") as HTMLInputElement;
        const cb5 = document.getElementById("5DayBox") as HTMLInputElement;
        const cb7 = document.getElementById("7DayBox") as HTMLInputElement;

        //Line graph grabber for later.
        const barGraphElement = document.getElementById("bGraph");

        const chart = Chart.getChart("bGraph")
        if (chart != null) { chart.destroy(); }

        //Define our Start/End variable and define the startime for use..
        const startTime = new Date().toLocaleDateString();
        let endTime

        //Check if none are checked(If so, the user is wrong.)
        if (cb1.checked == false && cb3.checked == false && cb5.checked == false && cb7.checked == false) { return; }

        //Toggle all other checkboxes off when the user clicks any of them and set our time variables.
        if (cb1.checked == true) {

            //Set our entime to current-7-1
            const subtractTime = new Date(new Date().getTime() - (1 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else if (cb3.checked == true) {

            //Set our entime to current-3
            const subtractTime = new Date(new Date().getTime() - (3 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else if (cb5.checked == true) {

            //Set our entime to current-5
            const subtractTime = new Date(new Date().getTime() - (5 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        }
        else {

            //Set our entime to current-7
            const subtractTime = new Date(new Date().getTime() - (7 * 24 * 60 * 60 * 1000));
            endTime = subtractTime.toLocaleDateString();
        };

        //Request the data based on our users selections.
        const requestStringLine = "Cycle?taskType=TimerData&startTime=" + startTime + "&endtime=" + endTime + "&timerName=" + searchText;
        const responseDataLine: string[] = await (await fetch(requestStringLine)).json();

        //Formats values for our for graphing.
        const barGraphLabels: string[] = [];
        const barGraphValues: number[] = [];
        let averageCalc: number = 0;
        let averageCalcFloat: number = 0;

        //Sort through the returned array and start populating the graphing data.
        responseDataLine.forEach((value) => {

            if (parseFloat(value.split("|")[1]) > 0 && parseFloat(value.split("|")[1]) < 200) {
                barGraphLabels.push(value.split("|")[0]);
                barGraphValues.push(parseFloat(value.split("|")[1]));
                averageCalc += 1;
                averageCalcFloat += parseFloat(value.split("|")[1]);
            }
        })

        averageCalcFloat = (averageCalcFloat / averageCalc) * 5;

        const refinedBarData = {
            labels: barGraphLabels,
            datasets: [{
                label: 'Timers History: ' + searchText,
                data: barGraphValues,
                backgroundColor: "blue",
                borderColor: "blue",
                borderWidth: 1
            }]
        };

        new Chart(
            // @ts-expect-error "Library issue that doesn't seem to be resolveable."
            barGraphElement,
            {
                type: 'bar',
                data: refinedBarData,
                options: {
                    scales: {
                        y: {
                            min: 0,
                            max: averageCalcFloat
                        },
                        x: {

                        }
                    }
                }
            }
        );
    }

    // #endregion

    // #region -Fault Information
    //Initial function for HTML5 code occurances.
    function Click_FaultInformation() {

        InitialErrorData(); 

        return ( 
            <div className="container">
                <thead><h1>Error Overlord</h1></thead>
                <form>

                <canvas id="barGraph" key="barGraph"></canvas>
                <br></br>
                    <label>Start Date:
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        End Date:
                    </label>
                    <br></br>

                    <input
                        type="date"
                        id="startSearchDate"
                        name="startSearchDate"
                        min="2000-06-07"
                        max="2099-06-07"
                        onInput={() => AutoGrabBarGraphs((document.getElementById("startSearchDate") as HTMLInputElement).value,
                            (document.getElementById("endSearchDate") as HTMLInputElement).value)}
                    />
                    <input
                        type="date"
                        id="endSearchDate"
                        name="endSearchDate"
                        min="2000-06-07"
                        max="2099-06-07"
                        onInput={() => AutoGrabBarGraphs((document.getElementById("startSearchDate") as HTMLInputElement).value,
                            (document.getElementById("endSearchDate") as HTMLInputElement).value)}
                    />
                    <br></br> <br></br>
                    <label>Choose An Error For Displaying:</label>
                    <select name="errorDisplay" id="errorDisplay" onInput={() => GraphTimeFrame((document.getElementById("startSearchDate") as HTMLInputElement).value,
                        (document.getElementById("endSearchDate") as HTMLInputElement).value,
                        (document.getElementById("errorDisplay") as HTMLInputElement).value)}>
                        <option value="PlaceHolder1">Loading... Please Wait...</option>
                    </select>
                    <br></br>

                    <canvas id="lineChart" key="lineChart"></canvas>

                    <div className="div-1" id="resultsBox">
                    </div>
                    <br></br>
                    <input type="submit" value="Go Back"></input>
                </form>
            </div>)
    }

    //Function that is run when the user clicks submit for a line graph.
    async function GraphTimeFrame(sTime: string, eTime: string, sFind: string) {

        //Refuse to graph if they made 0 selection.
        if (sTime == '' || eTime == '' || sFind == '') { return; }

        //Set our times to the last hours from today.
        const currentTime = new Date(eTime);
        const subtractTime = new Date(sTime);
        const startTime = new Date(subtractTime.setDate(subtractTime.getDate() + 1)).toLocaleDateString();
        const endTime = new Date(currentTime.setDate(currentTime.getDate() + 1)).toLocaleDateString();

        // #region "-Line Chart Start"
        //Format our request string.
        const requestStringLine = "Error?taskType=LineGraph&startTime=" + startTime + "&endTime=" + endTime + "&searchText=" + sFind;
   
        const responseDataLine: [string, string, number] = await (await fetch(requestStringLine)).json();

        //---Graphing Portion
        //Grab the initial values for the Graph
        const lineData: [name: string, value: number] = ["", 0];

        for (let i = 0; i < responseDataLine.length; i++) {

            const tuppleRecord = responseDataLine[i];
            // @ts-expect-error "Unknown why this occurs."
            const tuppleText = tuppleRecord.item1;
            // @ts-expect-error "Unknown why this occurs."
            const tuppleTime = tuppleRecord.item2;
            // @ts-expect-error "Unknown why this occurs."
            const tuppleValue = tuppleRecord.item3;

            //Check if our current array value matches the error the user wants selected.
            if (tuppleText == sFind) {
                lineData.push(tuppleTime, tuppleValue);
            }     
        }
        lineData.splice(0, 2);   //Delete our placeholder elements.

        const lineGraphElement = document.getElementById("lineChart"); //Grab Graphing For Mapping to.

        //Create Graphing Data
        const lineGraphLabels:string[] = [];
        const lineGraphValues = [];
        let lineGraphBorderColor: string = "";

        //Populate all our data.
        for (let j = 0; j < lineData.length; j++) {
            if (j % 2 == 0) { lineGraphLabels.push(lineData[j].toString()); }    //Refine into Bottom Labels
            if (j % 2 == 1) { lineGraphValues.push(lineData[j]); }    //Refine into Values

            //Random number generation
            const ColorValue1 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue2 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue3 = Math.floor(Math.random() * (255 - 1) + 1);

            //Set color values for bar.
            lineGraphBorderColor = "rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ")";
        }

        //Insert a 0 and 24 hour timestamp to fill in all missing hours.
        //Generate corrected start time.
        const splitStartTime = startTime.split("/");
        const formattedSplitStartTime = splitStartTime[2] + " " +
            splitStartTime[0].padStart(2, "0") + " " +
            splitStartTime[1].padStart(1, "0") + " 00";

        //Generate corrected end time.
        const splitEndTime = endTime.split("/");
        const formattedSplitEndTime = splitEndTime[2] + " " +
            splitEndTime[0].padStart(2, "0") + " " +
            splitEndTime[1].padStart(1, "0") + " 00"

        //Splice data for generation.
        lineGraphLabels.splice(0, 0, formattedSplitStartTime);
        lineGraphValues.splice(0, 0, 0);
        lineGraphLabels.splice(lineGraphLabels.length, 0, formattedSplitEndTime);
        lineGraphValues.splice(lineGraphLabels.length, 0, 0);

        //Generate in between values for all missing hours.
        for (let j = 0; j < lineGraphLabels.length-1; j++) {

            //Grab hour time differences to fill in additional points.
            const startTimeLength = lineGraphLabels[j].length;
            const endTimeLength = lineGraphLabels[j].length;

            //Grab the full name of the starting and ending labels.
            const startHour = Number(lineGraphLabels[j].substring(startTimeLength - 2, startTimeLength));
            const endHour = Number(lineGraphLabels[j+1].substring(endTimeLength - 2, endTimeLength));

            //Loop through anything that sees a > 1 hour time difference and start filling int the in between values.
            if (startHour + 1 < endHour) {
                let k = j;
                for (let i = startHour + 1; i < endHour; i++) {
                    k++;
                    const newHour = lineGraphLabels[k].substring(0, startTimeLength - 2) + " " + i.toString().padStart(2,"0");
                    lineGraphLabels.splice(k, 0, newHour);
                    lineGraphValues.splice(k, 0, 0);
                }
            }
        }

        //Generate a single point for our ending timestamp for visual clarity.
        if (lineGraphLabels[lineGraphLabels.length - 2].substring(lineGraphLabels.length - 2, lineGraphLabels.length) != "24") { 

            //Create our insertion string then add it along with a 0 value array.
            let newHour = lineGraphLabels[lineGraphLabels.length - 2];
            newHour = newHour.substring(0, newHour.length - 2) + Number(Number(newHour.substring(newHour.length - 2, newHour.length))+1);
                
            lineGraphLabels.splice(lineGraphValues.length - 1, 0, newHour);
            lineGraphValues.splice(lineGraphValues.length - 1, 0, 0);
        }

        //Generate a single point for our starting timestamp for visual clarity.
        if (lineGraphLabels[1].substring(lineGraphLabels.length - 2, lineGraphLabels.length) != "01") {

            //Create our insertion string then add it along with a 0 value array.
            let newHour = lineGraphLabels[1];
            newHour = newHour.substring(0, newHour.length - 2) + Number(Number(newHour.substring(newHour.length - 2, newHour.length)) - 1);

            lineGraphLabels.splice(1, 0, newHour);
            lineGraphValues.splice(1, 0, 0);
        }

        //Raw Graphing Data.
        const refinedLineData = {
            labels: lineGraphLabels,
            datasets: [{
                label: "Error History: " + sFind,
                data: lineGraphValues,
                borderColor: lineGraphBorderColor,
                tension: 0.1
            }]
        };

        //Delete the old chart if it exists.
        const chart = Chart.getChart("lineChart");
        chart?.destroy();

        new Chart(
            // @ts-expect-error "Library issue that doesn't seem to be resolveable."
            lineGraphElement,
            {
                type: 'line',
                data: refinedLineData,
            }
        );
         // #endregion
    }

    //Function that is run every time the user fills out a start/end date for the dynamic bargraph and error population.
    async function AutoGrabBarGraphs(sTime:string,eTime:string) {

        if (sTime == '' || eTime == '') { return; }

        //Set our times to the last hours from today.
        const currentTime = new Date(eTime);
        const subtractTime = new Date(sTime);
        const startTime = new Date(subtractTime.setDate(subtractTime.getDate() + 1)).toLocaleDateString();
        const endTime = new Date(currentTime.setDate(currentTime.getDate() + 1)).toLocaleDateString();

        //Format our request string.
        const requestString = "Error?taskType=BarGraph&startTime=" + startTime + "&endTime=" + endTime;

        //Grab the result then throw it into our response box.
        const responseData: [string, string, number] = await (await fetch(requestString)).json();

        //Fill our selection box the user.
        const selectionBox = document.getElementById("errorDisplay");
        //Clear Old Options
        while (selectionBox?.lastChild) {
            selectionBox?.removeChild(selectionBox.lastChild);
        }

        //Add a single empty selection so the user doesn't think they can just run stuff all willy nilly.
        const elementBlank = document.createElement("option");
        elementBlank.textContent = "";
        elementBlank.id = "TempSelections";
        selectionBox?.appendChild(elementBlank);

        //Add the rest of the users options.
        const addedElements: string[] = [];
        for (let i = 0; i < responseData.length; i++) {

            const tuppleRecord = responseData[i];
            // @ts-expect-error "Unknown why this occurs."
            const tuppleText = tuppleRecord.item1;

            if (addedElements.includes(tuppleText) == false) {
                const element = document.createElement("option");
                element.textContent = tuppleText;
                element.id = "TempSelections";
                selectionBox?.appendChild(element);
                addedElements.push(tuppleText);
            }
        }

        //---Graphing Portion
        //Grab the initial values for the Graph
        const barData: [name: string, value: number] = ["", 0];

        for (let i = 0; i < responseData.length; i++) {

            const tuppleRecord = responseData[i];
            // @ts-expect-error "Unknown why this occurs."
            const tuppleText = tuppleRecord.item1;
            // @ts-expect-error "Unknown why this occurs."
            const tuppleValue = tuppleRecord.item3;

            //The tupple list contains the value somewhere, loop through an update as needed.
            if (barData.includes(tuppleText)) {
                for (let j = 0; j < barData.length; j++) {
                    if (barData[j] == tuppleText) {
                        barData[j + 1] = barData[j + 1] + tuppleValue;
                    }
                }
            }
            else { barData.push(tuppleText, tuppleValue); } //Record isn't found, add it.            
        }
        barData.splice(0, 2);   //Delete our placeholder elements.

        const barGraphElement = document.getElementById("barGraph"); //Grab Graphing For Mapping to.


        const barGraphLabels = [{}]
        const barGraphValues = []
        const barGraphBorderColor = [{}]
        const barGraphBackgroundColor = [{}]

        //Populate all our data.
        for (let j = 0; j < barData.length; j++) {
            if (j % 2 == 0) { barGraphLabels.push(barData[j]); }    //Refine into Bottom Labels
            if (j % 2 == 1) { barGraphValues.push(barData[j]); }    //Refine into Values

            //Random number generation
            const ColorValue1 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue2 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue3 = Math.floor(Math.random() * (255 - 1) + 1);

            //Set color values for bar.
            barGraphBorderColor.push("rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ")");
            barGraphBackgroundColor.push("rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ", 0.2)");
        }

        //Remove the empty records at the start of our definitions.
        barGraphLabels.splice(0, 1);
        barGraphBorderColor.splice(0, 1);
        barGraphBackgroundColor.splice(0, 1);

        //Bubble Sorting Algorithm
        let temp: number
        let temp2: string
        for (let i = 0; i < barGraphValues.length; i++) {
            let swapped: boolean = false;
            for (let j = 0; j < barGraphValues.length - i; j++) {
                if (barGraphValues[j] < barGraphValues[j + 1]) {
                    //Store Temp Value
                    temp = Number(barGraphValues[j]);
                    temp2 = barGraphLabels[j].toString();

                    //Move Value Forward
                    barGraphValues[j] = barGraphValues[j + 1];
                    barGraphLabels[j] = barGraphLabels[j + 1];

                    //Put Temp in Moved Cell
                    barGraphValues[j + 1] = temp;
                    barGraphLabels[j + 1] = temp2;

                    swapped = true;
                }
            }
            if (swapped == false) { break; }
        }

        const refinedBarData = {
            labels: barGraphLabels,
            datasets: [{
                label: 'Errors Between: ' + sTime + ' And ' + eTime ,
                data: barGraphValues,
                backgroundColor: barGraphBackgroundColor,
                borderColor: barGraphBorderColor,
                borderWidth: 1
            }]
        };

        const chart = Chart.getChart("barGraph")
        if (chart != null) { chart.destroy(); }


        new Chart(
            // @ts-expect-error "Library issue that doesn't seem to be resolveable."
            barGraphElement,
            {
                type: 'bar',
                data: refinedBarData,
            }
        );
    }

    //Function that is called which generates all data on the server and fills out the last 24 hours worth of errors to a default graph.
    async function InitialErrorData() {

        let chart = Chart.getChart("barGraph")
        if (chart != null) { return; }

        //Set our times to the last hours from today.
        const currentTime = new Date();
        const subtractTime = new Date(new Date().getTime() - (1 * 24 * 60 * 60 * 1000));
        const startTime = subtractTime.toLocaleDateString();
        const endTime = currentTime.toLocaleDateString();
        
        //Format our request string.
        const requestString = "Error?taskType=Initial&startTime=" + startTime + "&endTime=" + endTime;

        //Grab the result then throw it into our response box.
        const responseData: [string, string, number] = await(await fetch(requestString)).json();

        //Fill our selection box the user.
        const selectionBox = document.getElementById("errorDisplay");
        //Clear Old Options
        while (selectionBox?.lastChild) {
            selectionBox?.removeChild(selectionBox.lastChild);
        }

        //Add a single empty selection so the user doesn't think they can just run stuff all willy nilly.
        const elementBlank = document.createElement("option");
        elementBlank.textContent = "";
        elementBlank.id = "TempSelections";
        selectionBox?.appendChild(elementBlank);

        //Add New Options
        const addedElements: string[] = [];
        for (let i = 0; i < responseData.length; i++) {

            const tuppleRecord = responseData[i];
            // @ts-expect-error "Unknown why this occurs."
            const tuppleText = tuppleRecord.item1;
            
            if (addedElements.includes(tuppleText) == false) {
                const element = document.createElement("option");
                element.textContent = tuppleText;
                element.id = "TempSelections";
                selectionBox?.appendChild(element);
                addedElements.push(tuppleText);
            }
        }    

        //---Graphing Portion
        //Grab the initial values for the Graph
        const barData: [name: string, value: number] = ["", 0];

        for (let i = 0; i < responseData.length; i++) {
 
                const tuppleRecord = responseData[i];
                // @ts-expect-error "Unknown why this occurs."
                const tuppleText = tuppleRecord.item1;
                // @ts-expect-error "Unknown why this occurs."
                const tuppleValue = tuppleRecord.item3; 

            //The tupple list contains the value somewhere, loop through an update as needed.
            if (barData.includes(tuppleText)) {
                for (let j = 0; j < barData.length; j++) {
                    if (barData[j] == tuppleText) {
                        barData[j + 1] = barData[j + 1] + tuppleValue;
                    }
                }
            }
            else { barData.push(tuppleText, tuppleValue); } //Record isn't found, add it.            
        }
        barData.splice(0, 2);   //Delete our placeholder elements.

        const barGraphElement = document.getElementById("barGraph"); //Grab Graphing For Mapping to.


        const barGraphLabels = [{}] 
        const barGraphValues = [] 
        const barGraphBorderColor = [{}]
        const barGraphBackgroundColor = [{}]

        //Populate all our data.
        for (let j = 0; j < barData.length; j++) {
            if (j % 2 == 0) { barGraphLabels.push(barData[j]); }    //Refine into Bottom Labels
            if (j % 2 == 1) { barGraphValues.push(barData[j]); }    //Refine into Values

            //Random number generation
            const ColorValue1 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue2 = Math.floor(Math.random() * (255 - 1) + 1);
            const ColorValue3 = Math.floor(Math.random() * (255 - 1) + 1);

            //Set color values for bar.
            barGraphBorderColor.push("rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ")");
            barGraphBackgroundColor.push("rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ", 0.2)");
        }

        //Remove the empty records at the start of our definitions.
        barGraphLabels.splice(0, 1);
        barGraphBorderColor.splice(0, 1);
        barGraphBackgroundColor.splice(0, 1);

        //Bubble Sorting Algorithm
        let temp: number
        let temp2: string
        for (let i = 0; i < barGraphValues.length; i++) {
            let swapped:boolean = false;
            for (let j = 0; j < barGraphValues.length - i; j++) {
                if (barGraphValues[j] < barGraphValues[j + 1]) {
                    //Store Temp Value
                    temp = Number(barGraphValues[j]);
                    temp2 = barGraphLabels[j].toString();

                    //Move Value Forward
                    barGraphValues[j] = barGraphValues[j + 1];
                    barGraphLabels[j] = barGraphLabels[j + 1];

                    //Put Temp in Moved Cell
                    barGraphValues[j + 1] = temp;
                    barGraphLabels[j + 1] = temp2;

                    swapped = true;
                }
            }
            if (swapped == false) { break; }
        }

        const refinedBarData = {
            labels: barGraphLabels,
            datasets: [{
                label: 'Errors Within The Last 24 Hours',
                data: barGraphValues,
                backgroundColor: barGraphBackgroundColor,
                borderColor: barGraphBorderColor,
                borderWidth: 1
            }]
        };

        chart = Chart.getChart("barGraph")
        if (chart != null) { chart.destroy(); }


        new Chart(
             // @ts-expect-error "Library issue that doesn't seem to be resolveable."
             barGraphElement,
             {
                 type: 'bar',
                 data: refinedBarData,
             }
         );
    }
    // #endregion

    // #region -Graph Information
    function Click_Graphs() {

        GraphWizardInitial()

        return (
            <div className="container">
                <thead><h1>Graph Wizard</h1></thead>
                <form>
                    <label>Start Date:
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        End Date:
                    </label>
                    <br></br>
                    <input
                        type="date"
                        id="startSearchDate"
                        name="startSearchDate"
                        min="2000-06-07"
                        max="2099-06-07"
                        onInput={() => WizardGetGraphs((document.getElementById("startSearchDate") as HTMLInputElement).value,
                            (document.getElementById("endSearchDate") as HTMLInputElement).value)}
                    />
                    <input
                        type="date"
                        id="endSearchDate"
                        name="endSearchDate"
                        min="2000-06-07"
                        max="2099-06-07"
                        onInput={() => WizardGetGraphs((document.getElementById("startSearchDate") as HTMLInputElement).value,
                            (document.getElementById("endSearchDate") as HTMLInputElement).value)}
                    />
                    <br></br>
                    <label>Choose Graph Data To Display:</label>
                    <select name="graphDisplay" id="graphDisplay" onInput={() => WizardProcessing((document.getElementById("startSearchDate") as HTMLInputElement).value,
                        (document.getElementById("endSearchDate") as HTMLInputElement).value,
                        (document.getElementById("graphDisplay") as HTMLInputElement).value)}>
                        <option value="PlaceHolder1">Choose TimeFrame And Graph...</option>
                    </select>
                    <br></br>

                    <canvas id="masterGraph" key="masterGraph"></canvas>
                    <canvas id="masterGraph2" key="masterGraph2"></canvas>
                    <canvas id="masterGraph3" key="masterGraph3"></canvas>
                    <canvas id="masterGraph4" key="masterGraph4"></canvas>
                    <br></br>
                    <input type="submit" value="Go Back"></input>
                </form>
            </div>)
    }

    async function GraphWizardInitial() {

        //Format our request string.
        const requestString = "GraphWizard?taskType=Initial";

        //Grab the result then throw it into our response box.
        await (await fetch(requestString)).json();

    }

    //Function that is run every time the user fills out a start/end date, this just populated our list of selections for the user..
    async function WizardGetGraphs(sTime: string, eTime: string) {

        if (sTime == '' || eTime == '') { return; }

        //Set our times to the last hours from today.
        const currentTime = new Date(eTime);
        const subtractTime = new Date(sTime);
        const startTime = new Date(subtractTime.setDate(subtractTime.getDate() + 1)).toLocaleDateString();
        const endTime = new Date(currentTime.setDate(currentTime.getDate() + 1)).toLocaleDateString();

        //Format our request string.
        const requestString = "GraphWizard?taskType=GraphNames&startTime=" + startTime + "&endTime=" + endTime;

        //Grab the result then throw it into our response box.
        const responseData: string = await (await fetch(requestString)).json();

        //Fill our selection box the user.
        const selectionBox = document.getElementById("graphDisplay");
        //Clear Old Options
        while (selectionBox?.lastChild) {
            selectionBox?.removeChild(selectionBox.lastChild);
        }

        //Add a single empty selection so the user doesn't think they can just run stuff all willy nilly.
        const elementBlank = document.createElement("option");
        elementBlank.textContent = "";
        elementBlank.id = "TempSelections";
        selectionBox?.appendChild(elementBlank);

        //Add the rest of the users options.
        for (let i = 0; i < responseData.length; i++) {

            const element = document.createElement("option");
            element.textContent = responseData[i];
            element.id = "TempSelections";
            selectionBox?.appendChild(element);
        }
    }

    //Function that is run when the user selects a graph
    async function WizardProcessing(sTime: string, eTime: string, sFind: string) {

        //Refuse to graph if they made 0 selection.
        if (sTime == '' || eTime == '' || sFind == '') { return; }

        //Set our times to the last hours from today.
        const currentTime = new Date(eTime);
        const subtractTime = new Date(sTime);
        const startTime = new Date(subtractTime.setDate(subtractTime.getDate() + 1)).toLocaleDateString();
        const endTime = new Date(currentTime.setDate(currentTime.getDate() + 1)).toLocaleDateString();

        //Form our data request and grab back our data.
        const requestString = "GraphWizard?taskType=GraphData&startTime=" + startTime + "&endTime=" + endTime + "&dataName=" + sFind;

        //Grab the result - Note data is fed back in the following format-
        const responseData: [string] = await (await fetch(requestString)).json();

        //Grab the type of graph we're doing.
        const graphType: string = responseData[0].toString()
        responseData.splice(0, 1);

        //--Destroy our old graphs if they exist.
        //Delete the old chart if it exists.
        const chart = Chart.getChart("masterGraph");
        chart?.destroy();

        //Delete the old chart if it exists.
        const chart2 = Chart.getChart("masterGraph2");
        chart2?.destroy();

        //Delete the old chart if it exists.
        const chart3 = Chart.getChart("masterGraph3");
        chart3?.destroy();

        //Delete the old chart if it exists.
        const chart4 = Chart.getChart("masterGraph4");
        chart4?.destroy();

        // #region -Line Graph Steps
        if (graphType == 'Line') {

            //Define our secondary arrays for ease of use.
            const timeOccurance = new Array<string>
            const valueName = new Array<string>
            const yValue = new Array<string>

            //Values that
            let lastName: string = ""
            let multPart: boolean = false
            let partCount: number = 0

            //Loop through and and break out our data for use.
            for (let i = 0; i < responseData.length; i++) {
                timeOccurance.push(responseData[i].split('|')[0].toString())
                valueName.push(responseData[i].split('|')[1].toString())
                yValue.push(responseData[i].split('|')[2].toString().replace('YVALUE=', ''))

                //Check if we have any data that indicates this is a multi part line graph.
                if (multPart == false) {
                    if (lastName != valueName[i] && lastName != "") { multPart = true }
                    lastName = valueName[i]
                }
            }

            //Values that we use if we have multiple lineGraphs on a singualar graph for use.
            const timeOccuranceMultiPart = new Array<string>
            const valueNameArrayMultiPart = new Array<string>
            const yValueArrayMultiPart = new Array<string>

            //Break out data into a multipart formatting for us to use.
            if (multPart == true) {

                //Determine all different names that there are, add them to our temp name array if they're not foud.
                const tempFoundNames = new Array<string>
                for (let i = 0; i < valueName.length; i++) {
                    if (!tempFoundNames.includes(valueName[i])) { tempFoundNames.push(valueName[i]) }
                }

                //Loop through and and break out our data for use based on ValueName.
                const tempStorageTime = new Array<string>; const tempStorageYValue = new Array<string>; const tempStorageValueName = new Array<string>
                for (let i = 0; i < tempFoundNames.length; i++) {

                    //Clear values between populates.
                    tempStorageTime.splice(0, tempStorageTime.length)
                    tempStorageYValue.splice(0, tempStorageYValue.length)
                    tempStorageValueName.splice(0, tempStorageValueName.length)

                    for (let j = 0; j < valueName.length; j++) {
                        if (tempFoundNames[i] == valueName[j]) {
                            tempStorageTime.push(timeOccurance[j])
                            tempStorageYValue.push(yValue[j])
                            tempStorageValueName.push(valueName[j])
                        }
                    }

                    //Populate our temporary string to our full one now.
                    for (let j = 0; j < tempStorageValueName.length; j++) {
                        timeOccuranceMultiPart.push(tempStorageTime[j])
                        valueNameArrayMultiPart.push(tempStorageValueName[j])
                        yValueArrayMultiPart.push(tempStorageYValue[j])
                    }

                    //Append our ending , for use.
                    timeOccuranceMultiPart.push(",")
                    valueNameArrayMultiPart.push(",")
                    yValueArrayMultiPart.push(",")

                    partCount += 1
                }

                //Delete our last unused , for our use.
                timeOccuranceMultiPart.splice(timeOccuranceMultiPart.length - 1, 1)
                valueNameArrayMultiPart.splice(valueNameArrayMultiPart.length - 1, 1)
                yValueArrayMultiPart.splice(yValueArrayMultiPart.length - 1, 1)
            }


            let arrayLength: number = 0

            //Determine which array we're  based on length.
            if (multPart == true) {
                arrayLength = timeOccuranceMultiPart.length
            }
            else {
                arrayLength = timeOccurance.length
            }

            //Format our DateTime for use from our raw string.
            for (let i = 0; i < arrayLength; i++) {

                //Insert in between characters to our required ones for a new datetime string.
                for (let j = 0; j < 5; j++) {
                    switch (j) {
                        case 0:
                            if ((multPart == true) && (!timeOccuranceMultiPart[i].includes(","))) { timeOccuranceMultiPart[i] = timeOccuranceMultiPart[i].replace(' ', '-') }
                            else if (!multPart) { timeOccurance[i] = timeOccurance[i].replace(' ', '-') };
                            break;
                        case 1:
                            if ((multPart == true) && (!timeOccuranceMultiPart[i].includes(","))) { timeOccuranceMultiPart[i] = timeOccuranceMultiPart[i].replace(' ', '-') }
                            else if (!multPart) { timeOccurance[i] = timeOccurance[i].replace(' ', '-') }
                            break;
                        case 2:
                            if ((multPart == true) && (!timeOccuranceMultiPart[i].includes(","))) { timeOccuranceMultiPart[i] = timeOccuranceMultiPart[i].replace(' ', 'T') }
                            else if (!multPart) { timeOccurance[i] = timeOccurance[i].replace(' ', 'T') }
                            break;
                        case 3:
                            if ((multPart == true) && (!timeOccuranceMultiPart[i].includes(","))) { timeOccuranceMultiPart[i] = timeOccuranceMultiPart[i].replace(' ', ':') }
                            else if (!multPart) { timeOccurance[i] = timeOccurance[i].replace(' ', ':') }
                            break;
                        case 4:
                            if ((multPart == true) && (timeOccuranceMultiPart[i] != ",")) { timeOccuranceMultiPart[i] = timeOccuranceMultiPart[i].replace(' ', ':') }
                            else if (!multPart) { timeOccurance[i] = timeOccurance[i].replace(' ', ':') }
                            break;
                    }
                }
            }

            const graphElement = document.getElementById("masterGraph"); //Grab Graphing For Mapping to.
            const graphElement2 = document.getElementById("masterGraph2"); //Grab Graphing For Mapping to.
            const graphElement3 = document.getElementById("masterGraph3"); //Grab Graphing For Mapping to.
            const graphElement4 = document.getElementById("masterGraph4"); //Grab Graphing For Mapping to.

            //--Add starting and ending values for readability.
            //Determine if we're running a single line or multiple lines on our graph.
            //Single Line Graph
            if (multPart == false) {
                //Create Starting T-.01/T-.02
                const firstTimePeriod: Date = new Date(timeOccurance[0]);
                const firstTimePeriodMinus1: Date = new Date(firstTimePeriod.getTime() - (1000))
                const firstTimePeriodMinus2: Date = new Date(firstTimePeriod.getTime() - (2000))

                //Create ending T+.01/T+.02
                const lastTimePeriod: Date = new Date(timeOccurance[timeOccurance.length - 1]);
                const lastTimePeriodPlus1: Date = new Date(lastTimePeriod.getTime() + (1000));
                const lastTimePeriodPlus2: Date = new Date(lastTimePeriod.getTime() + (2000));

                //Append To End
                timeOccurance.push(lastTimePeriodPlus1.toString());
                timeOccurance.push(lastTimePeriodPlus2.toString());
                yValue.push("0")
                yValue.push("0")

                //Append To Start
                timeOccurance.splice(0, 0, firstTimePeriodMinus1.toString())
                timeOccurance.splice(0, 0, firstTimePeriodMinus2.toString())
                yValue.splice(0, 0, "0")
                yValue.splice(0, 0, "0")

                //Raw Graphing Data.
                const refinedLineData = {
                    labels: timeOccurance,
                    datasets: [{
                        label: "Error History: " + sFind,
                        data: yValue.map(Number),
                        tension: 0.1
                    }]
                };

                new Chart(
                    // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                    graphElement,
                    {
                        type: 'line',
                        data: refinedLineData,
                    }
                );
            }

            //Multi Line Graph
            else {

                //Variables for parsing out our final data.
                const graphingArrayTime: Array<Array<string>> = new Array<Array<string>>
                const graphingArrayValue: Array<Array<string>> = new Array<Array<string>>
                const tempArray: Array<string> = new Array<string>
                const tempArray2: Array<string> = new Array<string>
                let lastCount: number = 0

                //Parse out our Array Data Based On Which Line it is.
                for (let j = 0; j < partCount; j++) {
                    for (let i = lastCount; i < timeOccuranceMultiPart.length; i++) {
                        if (timeOccuranceMultiPart[i] != ",") {
                            tempArray.push(timeOccuranceMultiPart[i])
                            tempArray2.push(yValueArrayMultiPart[i])
                        }
                        else {
                            lastCount = i + 1
                            break;
                        }
                    }

                    //Create Starting T-.01/T-.02
                    const firstTimePeriod: Date = new Date(tempArray[0]);
                    const firstTimePeriodMinus1: Date = new Date(firstTimePeriod.getTime() - (1000))
                    const firstTimePeriodMinus2: Date = new Date(firstTimePeriod.getTime() - (2000))

                    //Create ending T+.01/T+.02
                    const lastTimePeriod: Date = new Date(tempArray[tempArray.length - 1]);
                    const lastTimePeriodPlus1: Date = new Date(lastTimePeriod.getTime() + (1000));
                    const lastTimePeriodPlus2: Date = new Date(lastTimePeriod.getTime() + (2000));

                    //Append To End
                    tempArray.push(lastTimePeriodPlus1.toString());
                    tempArray.push(lastTimePeriodPlus2.toString());
                    tempArray2.push("0")
                    tempArray2.push("0")

                    //Append To Start
                    tempArray.splice(0, 0, firstTimePeriodMinus1.toString())
                    tempArray.splice(0, 0, firstTimePeriodMinus2.toString())
                    tempArray2.splice(0, 0, "0")
                    tempArray2.splice(0, 0, "0")

                    graphingArrayTime[j] = clone(tempArray)
                    graphingArrayValue[j] = clone(tempArray2)

                    tempArray.splice(0, tempArray.length)
                    tempArray2.splice(0, tempArray2.length)
                }

                //Create/Enable Graph 1
                if (graphingArrayTime.length >= 1) {

                    //Raw Graphing Data.
                    const refinedLineData = {
                        labels: graphingArrayTime[0],
                        datasets: [{
                            label: "Data1: " + sFind,
                            data: graphingArrayValue[0].map(Number),
                            tension: 0.1
                        }]
                    };

                    new Chart(
                        // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                        graphElement,
                        {
                            type: 'line',
                            data: refinedLineData,
                        }
                    );
                };

                //Create/Enable Graph 2
                if (graphingArrayTime.length >= 2) {

                    //Raw Graphing Data.
                    const refinedLineData = {
                        labels: graphingArrayTime[1],
                        datasets: [{
                            label: "Data2: " + sFind,
                            data: graphingArrayValue[1].map(Number),
                            tension: 0.1
                        }]
                    };

                    new Chart(
                        // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                        graphElement2,
                        {
                            type: 'line',
                            data: refinedLineData,
                        }
                    );
                };

                //Create/Enable Graph 3
                if (graphingArrayTime.length >= 3) {

                    //Raw Graphing Data.
                    const refinedLineData = {
                        labels: graphingArrayTime[2],
                        datasets: [{
                            label: "Data3: " + sFind,
                            data: graphingArrayValue[2].map(Number),
                            tension: 0.1
                        }]
                    };

                    new Chart(
                        // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                        graphElement3,
                        {
                            type: 'line',
                            data: refinedLineData,
                        }
                    );
                };

                //Create/Enable Graph 4
                if (graphingArrayTime.length >= 4) {

                    //Raw Graphing Data.
                    const refinedLineData = {
                        labels: graphingArrayTime[3],
                        datasets: [{
                            label: "Data4: " + sFind,
                            data: graphingArrayValue[3].map(Number),
                            tension: 0.1
                        }]
                    };

                    new Chart(
                        // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                        graphElement4,
                        {
                            type: 'line',
                            data: refinedLineData,
                        }
                    );
                };
            }
        }
        // #endregion

        // #region -Bar Graph Steps
        if (graphType == 'Bar') {

            const dataNames: string[] = []
            const dataCount: number[] = [0, 0, 0]
            const barColors: string[] = []

            //Grab the value names of each item from our response
            for (let i = 0; i < responseData.length; i++) {

                //Grab the name and see if our list contaisn it, if not, add it.
                const tempName: string = responseData[i].split('|')[1].toString().replace('VALUENAME=','')

                if (!dataNames.includes(tempName)) { dataNames.push(tempName) }   
            }

            //Now go through and get our counts for each valuetype.
            for (let i = 0; i < dataNames.length; i++) {
                for (let j = 0; j < responseData.length; j++) {
                    if (responseData[j].includes(dataNames[i])) { dataCount[i]++ }
                }

                //Also generate our bar graph colors while we're at it
                //Random number generation
                const ColorValue1 = Math.floor(Math.random() * (255 - 1) + 1);
                const ColorValue2 = Math.floor(Math.random() * (255 - 1) + 1);
                const ColorValue3 = Math.floor(Math.random() * (255 - 1) + 1);

                //Set color value for bar.
                barColors.push("rgba(" + ColorValue1.toString() + ", " + ColorValue2.toString() + ", " + ColorValue3.toString() + ")");
            }

            const graphElement = document.getElementById("masterGraph"); //Grab Graphing For Mapping to.

            const refinedBarData = {
                labels: dataNames,
                datasets: [{
                    label: 'Graph Values:' + sFind,
                    data: dataCount,
                    backgroundColor: barColors,
                    borderColor: barColors,
                    borderWidth: 1
                }]
            };

            new Chart(
                // @ts-expect-error "Library issue that doesn't seem to be resolveable."
                graphElement,
                {
                    type: 'bar',
                    data: refinedBarData,
                }
            );
 
        }
        //#endregion
    }
    // #endregion

    // #region -Log Search
    function Click_LogSearch() {
        return (
            <div className="container">
                <thead><h1>Machine Log Search</h1></thead>
                <form>
                    <label>Start Date:
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        End Date:
                    </label>
                    <br></br>
                    <input
                        type="date"
                        id="startSearchDate"
                        name="startSearchDate"
                        min="2000-06-07"
                        max="2099-06-07" 
                        onChange={() => setStartSearchDate((document.getElementById("startSearchDate") as HTMLInputElement).value)}/>
                    <input
                        type="date"
                        id="endSearchDate"
                        name="endSearchDate"
                        min="2000-06-07"
                        max="2099-06-07"
                        onChange={() => setEndSearchDate((document.getElementById("endSearchDate") as HTMLInputElement).value) }/>
                    <br></br> <br></br>
                    <label>Search For:</label>
                    <br></br>
                    <input type="text" id="searchFor" name="searchFor" onInput={() => setSearchFor((document.getElementById("searchFor") as HTMLInputElement).value)}></input>
                    <br></br>
                    <label>Results:</label>
                    <div className="div-1" id= "resultsBox">
                    </div>
                    <br></br>
                    <input type="button" value="Search" onClick={() => UpdateSearchResults(searchFor, startSearchDate, endSearchDate, document.getElementById("resultsBox") as HTMLDivElement) }></input>
                    <input type="button" value="Copy Text" onClick={()=>CopyToClipboard() }></input>
                    <input type="submit" value="Go Back"></input>
                </form>
            </div>)
    }

    //Function that sends and updates our search results.
    async function UpdateSearchResults(searchString:string, startDate:string, endDate:string, resultBox: HTMLDivElement) {

        //Format our request string.
        const requestString = "LogSearch?searchText=" + searchString +
            "&sd=" + startDate +
            "&ed=" + endDate;

        //Grab the result then throw it into our response box.
        const responseData = await (await fetch(requestString)).json();

        resultBox.textContent = ""; //Clear out our old text contents.

        //Loop through and populate each list return to the search box for everyone.
        for (let i = 0; i < responseData.length; i++) {
            resultBox.textContent += responseData[i];
        }    
    }

    async function CopyToClipboard() {

        // Get the text field and contents.
        const textObject = document.getElementById("resultsBox");
        const copyText = textObject?.textContent;

        //Throw it into the clip buffer and call it a day.
        navigator.clipboard.writeText(copyText!.toString());
    }
    

    // #endregion

    // #region -FileBrowser
    function Click_FileBrowser() {

        InitialDriveContents()

        return (
                <div className="container">
                    <thead><h1>File Rogue</h1></thead>
                    <form>
                    <div className="div-2">
                        <button id="explorerButton" type="button" onClick={() => GoBack()}>
                                <img className="explorerImage" src="\src\Page_Main\Images\LeftArrow.png"></img>
                            </button>
                        <button id="explorerButton" type = "button" onClick={() => GoForward()}>
                                <img className="explorerImage" src="\src\Page_Main\Images\RightArrow.png"></img>
                            </button>
                        <button id="explorerButton" type="button" onClick={() => GoHome()}>
                            <img className="explorerImage" src="\src\Page_Main\Images\UpArrow.png"></img>
                        </button>
                        <input type="text" id="explorerPath"></input>
                    </div>
                    <br></br>
                    <div align-content="left">

                        {/*@ts-expect-error/ Size attribute is picked up incorrectly when it requires a string for this.*/}
                        <select id="fileSelector" onDoubleClick={() => FormNewRequest()} height="512px" size="30">
                            <option>
                                Please Wait, loading...
                            </option>
                        </select>
                    </div>
                    <input type="submit" value="Go Back"></input>
                    </form>
                </div>)
    }

    async function InitialDriveContents() {

        //Check if this function was already called.
        if (initializedFolder == "true") { return; }
        setInitFolder("true");

        //Format our request string.
        const requestString = "FileExplorer?folderLocation=C:\\";

        //Grab the result then throw it into our response box.
        const responseData: string = await (await fetch(requestString)).json();

        //Set the current file path.
        setCurrentFolder("C:");

        const pathBar = document.getElementById("explorerPath") as HTMLInputElement;
        pathBar!.value = "C:\\";

        //Grab our main listing box to sort through.
        const optionList = document.getElementById("fileSelector");

        //Clear Old Options
        while (optionList?.lastChild) {
            optionList?.removeChild(optionList.lastChild);
        }

        //Add New Options
        for (let i = 0; i < responseData.length; i++) {

            const element = document.createElement("option");
            if (responseData[i].substring(responseData[i].lastIndexOf('\\') + 1, responseData[i].length - responseData[i].lastIndexOf('\\') + 2).includes('.')) {
                element.textContent = "📝" + responseData[i].substring(responseData[i].lastIndexOf('\\') + 1, responseData[i].length - responseData[i].lastIndexOf('\\') + 2);
            }
            else
            {
                element.textContent = "📁" +  responseData[i].substring(responseData[i].lastIndexOf('\\') + 1, responseData[i].length - responseData[i].lastIndexOf('\\') + 2);
            }
            element.id = "fileBrowse1";
            optionList?.appendChild(element);
        }    
    }

    async function FormNewRequest() {

        //Form our new filepath then request a new user selection based on it.
        const newSelection = (document.getElementById("fileSelector") as HTMLInputElement).value
        const newPathCleaned = currentFolder + "\\" + newSelection.replace('📝', '').replace('📁','')

        if (!newPathCleaned.includes('.')) {
            UserNewSelection(newPathCleaned);
        }
        else {
            DownloadServerFile(newPathCleaned);
        }
        
    }

    async function GoBack() {

        //Get the current folder path we're at.
        const pathBar = document.getElementById("explorerPath") as HTMLInputElement;
        let folderPath = pathBar!.value

        setForwardFolder(folderPath)

        //Check if we have a "\ on the end of the filepath and delete it if so."
        if (folderPath.substring(folderPath.length - 1, folderPath.length) == '\\')
        {
            folderPath = folderPath.substring(0, folderPath.length - 1)
        }

        //Use a trimmed path to go back.
        UserNewSelection(folderPath.substring(0, folderPath.lastIndexOf('\\')) + '\\')
    }

    async function GoForward() {
        UserNewSelection(forwardFolder)
    }

    async function GoHome() {
        setInitFolder("false")
        GenerateInitialData()
    }

    async function UserNewSelection(folderPath: string) {

        //Format our request string.
        const requestString = "FileExplorer?folderLocation=" + folderPath;

        //Grab the result then throw it into our response box.
        const responseData: string = await (await fetch(requestString)).json();

        //Update folder path and the address bar to the new one.
        if (folderPath.includes(".") == false) {
            setCurrentFolder(folderPath);

            const pathBar = document.getElementById("explorerPath") as HTMLInputElement;
            pathBar!.value = folderPath;
        }

        //Grab our main listing box to sort through.
        const optionList = document.getElementById("fileSelector");

        //Clear Old Options
        while (optionList?.lastChild) {
            optionList?.removeChild(optionList.lastChild);
        }

        //Add New Options
        for (let i = 0; i < responseData.length; i++) {

            const itemName = responseData[i].substring(responseData[i].lastIndexOf('\\') + 1, responseData[i].length);

            const element = document.createElement("option");
            if (itemName.includes('.')) {
                element.textContent = "📝" + itemName;
            }
            else {
                element.textContent = "📁" + itemName;
            }
            element.id = "fileBrowse1";
            optionList?.appendChild(element);
        }
    }

    async function DownloadServerFile(filePath: string) {

        //Format our request string.
        const requestString = "Download?fileLocation=" + filePath;

        //Grab the result then throw it into our response blob.
        const responseData = await (await fetch(requestString)).json();
        const blob: Blob = new Blob([atob(responseData)]);  //Decode to Base64(Huge Pain)

        //Create and download the file by forcing a self click.
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = filePath.substring(filePath.lastIndexOf('\\') + 1, filePath.length);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    // #endregion

    // #region -Station Information
    function Click_StationInfo() {

        GetStationImage()

        return (
            <div>
                <img id="desktopImage" src=""></img>
                <br></br>
                <form><input type="submit" value="Go Back"></input></form>
            </div>
        )
    }

    ///Grabs the initial image when you go to this page for the station and displays it.
    async function GetStationImage() {
        //Format our request string.
        const requestString = "DesktopImage";

        //Grab the result then throw it into our response box.
        const responseData = await (await fetch(requestString)).text()

        //Display the Image
        const docImage = document.getElementById("desktopImage") as HTMLImageElement
        docImage.src = "data:image/jpeg;base64," + responseData
    }
    // #endregion

    // #region -Support Page 
    //-Main Page Button Click
    function Click_Support() {

        return (
            <div className="container">
                {/*---Contact Support*/}
                < thead >
                    <h1>PECD Station Manager</h1>
                </thead >

                {/*---Entry Form Start*/}
                <form>
                    <label>First Name</label>
                    <input type="text" id="fname" name="firstname" placeholder="Your name.." onInput={() => setFirstName((document.getElementById("fname") as HTMLInputElement).value)}></input>

                    <label>Last Name</label>
                    <input type="text" id="lname" name="lastname" placeholder="Your last name.." onInput={() => setLastName((document.getElementById("lname") as HTMLInputElement).value)} ></input>

                    <label>Email Address</label>
                    <input type="text" id="email" name="email" placeholder="PlaceHolder@aol.com" onInput={() => setEmailAddress((document.getElementById("email") as HTMLInputElement).value)} ></input>

                    <label>Subject</label>
                    <textarea id="body" name="subject" placeholder="Write something.." onInput={() => setTextBody((document.getElementById("body") as HTMLInputElement).value)} ></textarea>

                    <input type="submit" value="Cancel"></input>
                    <input type="submit" value="Submit" onClick={() => fetch("Email" + "?firstName=" + firstName + "&lastName=" + lastName + "&emailAddress=" + emailAddress + "&bodyText=" + textBody)}></input>
                </form>
            </div>
        )
    }
    // #endregion
}



export default App;