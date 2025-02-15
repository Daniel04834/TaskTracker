// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/*var userId = document.getElementById("user_id").innerText;
var sessionKey = document.getElementById("unique_id").innerText;
document.getElementById("user_id").remove();
document.getElementById("unique_id").remove();*/
//import Cookies from 'js-cookie'

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
        //console.log(element);
        //console.log(tasks.length-index);
        var newOrder = tasks.length-index;
        console.log(element);
        console.log(newOrder);
        if(element.getAttribute("task-order") == newOrder && element.getAttribute("task-column-id") == columnId) continue;
        if(element.getAttribute("task-column-id") != columnId) element.setAttribute("task-column-id", columnId);
        console.log("Pass");
        element.setAttribute("task-order", newOrder);
        console.log("TASK-ID: " + element.getAttribute("task-id"));
        //updatedTasks[element.getAttribute("task-id")] = newOrder;
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
    //const expires = date.toUTCString();
    //document.cookie = `${name}=${value}; expires=${expires}; path=/`;
    Cookies.set(name, value, { expires: date });
    console.log("Cookie set: " + document.cookie);
}

function Delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
/*function GetCookie(name){
    const cookie = document.cookie.split(";").find(cookie => cookie.includes(name));
    return cookie ? cookie.split("=")[1] : null;
}*/