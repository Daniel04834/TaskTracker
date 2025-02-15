var projectId = FindGetParameter("ProjectId");


window.onload = function(){
    if(window.location.href.includes("ProjectId")) {
        TaskRefresher();
        ColumnRefresher();
        RegisterDragDrop();
    }
    document.getElementById("add-new-column").addEventListener("click", NewColumnEvent);
}

function NewTaskEvent(columnId){
    var order = 0
    try {
        order = 1+parseInt(Array.from(document.querySelector(`[column-id="${columnId}"]`).querySelectorAll(`[task-order]`)).sort((a, b) => b.getAttribute("task-order") - a.getAttribute("task-order"))[0].getAttribute("task-order"));
    }
    catch{}
    const xhttp = new XMLHttpRequest();
    xhttp.open("POST", `https://${document.location.host}/api/project/${projectId}/task/new?columnId=${columnId}&title=change-me&order=${order}`, true);
    xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
    xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
    xhttp.send();
}

function NewColumnEvent(){
    const xhttp = new XMLHttpRequest();
    xhttp.open("POST", `https://${document.location.host}/api/project/${projectId}/column/new?title=change-me`, true);
    xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
    xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
    xhttp.send();
}

function ReassignTaskOrdersAndColumns(columnId){
    var updatedTasks = [];

    var taskColumn = document.querySelector(`[column-id="${columnId}"]`);
    var tasks = Array.from(taskColumn.getElementsByTagName("task-card"));
    for (let index = 0; index < tasks.length; index++) {
        const element = tasks[index];
        var newOrder = tasks.length-index;
        if(element.getAttribute("task-order") == newOrder && element.getAttribute("task-column-id") == columnId) continue;
        if(element.getAttribute("task-column-id") != columnId) element.setAttribute("task-column-id", columnId);
        element.setAttribute("task-order", newOrder);
        updatedTasks.push(element.getAttribute("task-id"));
    }
    
    return updatedTasks;
}

function RearrangeTasksByOrder(columnId){
    var taskColumn = document.querySelector(`[column-id="${columnId}"]`);
    var taskArea = taskColumn.getElementsByClassName("tasks-area")[0];
    var tasks = Array.from(taskColumn.getElementsByTagName("task-card"));
    for (let index = 0; index < tasks.length; index++) {
        const element = tasks[index];
        taskArea.removeChild(element);
    }
    tasks = tasks.sort((a, b) => b.getAttribute("task-order") - a.getAttribute("task-order"));
    tasks.forEach(element => {
        taskArea.appendChild(element);
    });
}

function MoveTaskToColumn(taskId, columnId) {
    var taskColumn = document.querySelector(`[column-id="${columnId}"]`);
    var taskArea = taskColumn.getElementsByClassName("tasks-area")[0];
    var taskElement = document.querySelector(`[task-id="${taskId}"]`);
    
    if(!taskArea.contains(taskElement)) taskArea.append(taskElement);
}

function FindGetParameter(parameterName) {
    var result = null,
        tmp = [];
    location.search
        .substr(1)
        .split("&")
        .forEach(function (item) {
          tmp = item.split("=");
          if (tmp[0] === parameterName) result = decodeURIComponent(tmp[1]);
        });
    return result;
}

function SetCookie(name, value, expiresMinutes){
    const date = new Date();
    date.setTime(date.getTime() + (expiresMinutes * 60 * 1000)); // minutes to milliseconds
    Cookies.set(name, value, { expires: date });
}

function Delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}