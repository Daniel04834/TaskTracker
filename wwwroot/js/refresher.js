var refreshInterval = 1000;

var updated = [];
var taskColumns = document.getElementById("task-columns");

function TaskRefresher() {
    setInterval(async () => {
        var jsonUpdatedTasks = GetUpdatedTasks();
        var columnsToUpdate = new Set();
        Object.keys(jsonUpdatedTasks).forEach(key => {
            if(updated.includes(key)) return;

            updated.push(key);

            var element = jsonUpdatedTasks[key];
            var taskElement = document.querySelector(`[task-id="${element.id}"]`);
            if(taskElement == null) {
                var taskColumnArea = document.querySelector(`[column-id="${element.columnId}"]`).getElementsByClassName("column")[0].getElementsByClassName("tasks-area")[0];
                var taskCard = document.createElement("task-card");
                taskCard.setAttribute("task-id", element.id);
                taskCard.setAttribute("task-column-id", element.columnId);
                taskCard.setAttribute("task-title", element.title);
                taskCard.setAttribute("task-order", element.order);
                taskCard.setAttribute("task-completed", element.completed);
                taskCard.setAttribute("task-collaborators", element.collaborators);
                taskCard.setAttribute("animation", "animate__fadeInUp");
                taskColumnArea.prepend(taskCard);
            }
            else{
                if(taskElement.getAttribute("task-column-id") != element.columnId) 
                { 
                    columnsToUpdate.add(element.columnId); 
                    columnsToUpdate.add(taskElement.getAttribute("task-column-id"));
                    taskElement.setAttribute("task-column-id", element.columnId);
                }
                if(taskElement.getAttribute("task-title") != element.title) taskElement.setAttribute("task-title", element.title);
                if(taskElement.getAttribute("task-order") != element.order) { columnsToUpdate.add(element.columnId); taskElement.setAttribute("task-order", element.order); }
                if(taskElement.getAttribute("task-completed").toLowerCase() != element.completed) taskElement.setAttribute("task-completed", element.completed);
            }
        });
        await Delay(500);
        columnsToUpdate.forEach(columnId => {
            RearrangeTasksByOrder(columnId);
        });
        columnsToUpdate.clear();
    }, refreshInterval);
}

function ColumnRefresher() {
    setInterval(() => {
        var jsonUpdatedColumns = GetUpdatedColumns();
        Object.keys(jsonUpdatedColumns).forEach(key => {
            if(updated.includes(key)) return;

            updated.push(key);

            var element = jsonUpdatedColumns[key];
            var columnElement = document.querySelector(`[column-id="${element.id}"]`);
            if(columnElement == null) {
                var taskColumns = document.getElementById("task-columns");
                var taskColumn = document.createElement("task-column");
                taskColumn.setAttribute("column-id", element.id);
                taskColumn.setAttribute("column-title", element.title);
                taskColumn.setAttribute("column-order", element.order);
                taskColumns.insertBefore(taskColumn, document.getElementById("add-new-column"));
            }
            else{
                columnElement.setAttribute("column-title", element.title);
            }
        });
    }, refreshInterval);
}

function GetUpdatedColumns(){
    const xhttp = new XMLHttpRequest();
    xhttp.open("GET", `https://${document.location.host}/api/project/${projectId}/column/get/updated`, false);
    xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
    xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
    xhttp.send();
    while (!xhttp.DONE) Delay(100);

    return xhttp.status == 200 ? JSON.parse(xhttp.responseText) : null;
}

function GetUpdatedTasks(){
    const xhttp = new XMLHttpRequest();
    xhttp.open("GET", `https://${document.location.host}/api/project/${projectId}/task/get/updated`, false);
    xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
    xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
    xhttp.send();
    while (!xhttp.DONE) Delay(100);

    return xhttp.status == 200 ? JSON.parse(xhttp.responseText) : null;
}