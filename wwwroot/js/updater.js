var sortable = null;

function InitializeNewTask(taskId) {
    console.log("Initializing new task");
    var taskElement = document.querySelector(`[task-id="${taskId}"]`);
    var input = taskElement.getElementsByTagName("input")[0]

    var valueBefore = input.value;
    input.addEventListener("focusin", () => {
        console.log("Focus");
        valueBefore = input.value;
    });
    input.addEventListener("blur", () => {
        if(valueBefore == input.value) return;
        console.log(input.value);
        const xhttp = new XMLHttpRequest();
        xhttp.open("PATCH", `https://${document.location.host}/api/project/${projectId}/task/${taskId}/update?title=${input.value}`, true);
        xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
        xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
        xhttp.send();
    });

    var checkmark = taskElement.getElementsByClassName("checkmark-icon")[0];
    checkmark.addEventListener("click", () => {
        /*if(!checkmark.classList.contains("checkmark-icon-checked")) checkmark.classList.add("checkmark-icon-checked");
        else checkmark.classList.remove("checkmark-icon-checked");*/
        var completed = taskElement.getAttribute("task-completed").toLowerCase() != "true";
        taskElement.setAttribute("task-completed", completed);
        const xhttp = new XMLHttpRequest();
        xhttp.open("PATCH", `https://${document.location.host}/api/project/${projectId}/task/${taskId}/update?completed=${completed}`, true);
        xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
        xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
        xhttp.send();
    });
}
function InitializeNewColumn(columnId) {
    console.log("Initializing new column");
    var columnElement = document.querySelector(`[column-id="${columnId}"]`);
    var input = columnElement.getElementsByTagName("input")[0]

    var valueBefore = input.value;
    input.addEventListener("focusin", () => {
        console.log("Focus");
        valueBefore = input.value;
    });
    input.addEventListener("blur", () => {
        if(valueBefore == input.value) return;
        console.log(input.value);
        const xhttp = new XMLHttpRequest();
        xhttp.open("PATCH", `https://${document.location.host}/api/project/${projectId}/column/${columnId}/update?title=${input.value}`, true);
        xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
        xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
        xhttp.send();
    });
}

function RegisterDragDrop(){
    if(sortable != null) sortable.destroy();
    sortable = new Draggable.Sortable(document.querySelectorAll(".tasks-area"), {
        draggable: "task-card",
        distance: 20,
        classes: {
            "source:dragging": ["draggable-source--is-dragging", "animate__animated", "animate__flash", "animate__infinite"],
            "mirror": ["opacity-50"],
          },
    });
    sortable.on('sortable:stop', async (e) => {
        await Delay(1);
        //console.log(e);
        var columnId1 = e.data.oldContainer.closest("task-column").getAttribute("column-id");
        var columnId2 = e.data.newContainer.closest("task-column").getAttribute("column-id");
        var updatedTasks = ReassignTaskOrdersAndColumns(columnId1);
        if(columnId1 != columnId2) updatedTasks = updatedTasks.concat(ReassignTaskOrdersAndColumns(columnId2));
        console.log("updated: ");
        console.log(updatedTasks);
        //await Delay(5000)
        updatedTasks.forEach(taskId => {
            var taskElement = document.querySelector(`[task-id="${taskId}"]`);
            var order = taskElement.getAttribute("task-order");
            var columnId = taskElement.getAttribute("task-column-id");
            const xhttp = new XMLHttpRequest();
            xhttp.open("PATCH", `https://${document.location.host}/api/project/${projectId}/task/${taskId}/update?order=${order}&columnId=${columnId}`, true);
            xhttp.setRequestHeader("user-id", Cookies.get("user-id"));
            xhttp.setRequestHeader("x-api-key", Cookies.get("unique-id"));
            xhttp.send();
    });
    });
}
