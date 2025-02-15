class TaskColumn extends HTMLElement {
    static observedAttributes = ["column-title"];

    constructor() {
      super();
    }

    connectedCallback() {
        var id = this.getAttribute("column-id");
        var title = this.getAttribute("column-title");
        var order = this.getAttribute("column-order");

        var before = this.innerHTML;
        this.innerHTML = `<div class="column">
            <input class="title w-64 text-2xl font-semibold text-center" style="background-color: unset;" value="${title}" />
            <span class="new-task cursor-pointer">+</span>
            <div class="tasks-area h-16">
                ${before}
            </div>
        </div>`;

        InitializeNewColumn(id);
        this.getElementsByClassName("new-task")[0].addEventListener("click", () => NewTaskEvent(id));
        RegisterDragDrop();
    }
    
    attributeChangedCallback(name, oldValue, newValue) {
        console.log(
          `Attribute ${name} has changed from ${oldValue} to ${newValue}.`,
        );
        if(name == "column-title"){
            this.getElementsByClassName("title")[0].value = newValue;
        }
      }
}

customElements.define('task-column', TaskColumn);