class TaskCard extends HTMLElement {
    static observedAttributes = ["task-column-id", "task-title", "task-order", "task-completed"];

    constructor() {
      super();
    }

    connectedCallback() {
        var id = this.getAttribute("task-id");
        var columnId = this.getAttribute("task-column-id");
        var title = this.getAttribute("task-title");
        var order = this.getAttribute("task-order");
        var completed = this.getAttribute("task-completed").toLowerCase() == "true";
        var collaborators = this.getAttribute("task-collaborators");

        var animation = this.getAttribute("animation") == null ? "animate__fadeIn" : this.getAttribute("animation");

        this.innerHTML = `<div class="animate__animated ${animation} relative flex flex-col mt-2 min-h-20 h-20 w-64 rounded-md bg-white px-3 py-2 ring-1 shadow-[0px_0px_5px_0px_rgba(0,0,0,255)] ring-gray-300 transition-all duration-300 hover:shadow-[0px_0px_5px_2px_rgba(0,0,0,255)]" style="font-family: 'Mulish', serif;">
            <div class="flex items-center">
                <svg class="checkmark-icon ${completed ? `checkmark-icon-checked` : ``}" width="17" height="17" viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g id="Checkmark">
                        <g id="Circle" filter="url(#filter0_d_1_17)">
                            <circle cx="50" cy="50" r="45" fill="#FFF" />
                            <circle cx="50" cy="50" r="46.5" stroke="black" stroke-width="3" />
                        </g>
                        <path id="Mark" d="M17 43.3333L43.8 70L84 30" stroke="black" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
                    </g>
                    <defs>
                        <filter id="filter0_d_1_17" x="1" y="1" width="98" height="98" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                            <feFlood flood-opacity="0" result="BackgroundImageFix" />
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha" />
                            <feOffset />
                            <feGaussianBlur stdDeviation="0.5" />
                            <feComposite in2="hardAlpha" operator="out" />
                            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0" />
                            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1_17" />
                            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1_17" result="shape" />
                        </filter>
                    </defs>
                </svg>
                <input class="title ml-1" value="${title}" />
            </div>

            <!--<div class="flex grow items-end">
                <div class="absolute left-3 mx-auto h-6 w-6 rounded-[100%] bg-[url('https://media.npr.org/assets/img/2021/11/21/gettyimages-1354349553_wide-e0acd4484282c66ca256064535b49f8bf29374af.jpg?s=1400&c=100&f=jpeg')] bg-cover bg-center"></div>
                <div class="absolute left-6 mx-auto h-6 w-6 rounded-[100%] bg-[url('https://hatrabbits.com/wp-content/uploads/2017/01/random.jpg')] bg-cover bg-center"></div>
                <div class="absolute left-9 mx-auto h-6 w-6 rounded-[100%] bg-[url('https://i.redd.it/i-got-bored-so-i-decided-to-draw-a-random-image-on-the-v0-4ig97vv85vjb1.png?width=1280&format=png&auto=webp&s=7177756d1f393b6e093596d06e1ba539f723264b')] bg-cover bg-center"></div>
            </div>-->
        </div>`;
        if(completed) this.classList.add("opacity-[60%]");

        InitializeNewTask(id);
    }
    
    attributeChangedCallback(name, oldValue, newValue) {
        console.log(
          `Attribute ${name} has changed from ${oldValue} to ${newValue}.`,
        );
        if(name == "task-column-id"){
            if(this.parentElement != null) MoveTaskToColumn(this.getAttribute("task-id"), newValue);
        }
        if(name == "task-title"){
            this.getElementsByClassName("title")[0].value = newValue;
        }
        if(name == "task-order"){
            //RearrangeTasksByOrder(this.getAttribute("task-column-id"));
        }
        if(name == "task-completed"){
            var checkmark = this.getElementsByClassName("checkmark-icon")[0];
            if(newValue.toLowerCase() == "true"){ 
                checkmark.classList.add("checkmark-icon-checked");
                this.classList.add("opacity-[60%]");
            }
            else{ 
                checkmark.classList.remove("checkmark-icon-checked");
                this.classList.remove("opacity-[60%]");
            }
        }
    }
}

customElements.define('task-card', TaskCard);