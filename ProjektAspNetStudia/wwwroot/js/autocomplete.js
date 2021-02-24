const createAutoCompleteList = (dataArr, inputElem) => {
    const list = document.createElement('div');
    list.classList.add('autocomplete-items', 'w-75');

    dataArr.forEach(elem => {
        const listElem = document.createElement('div');

        const text = document.createTextNode(elem.username);

        const hiddenInput = document.createElement('input');
        hiddenInput.setAttribute('type', 'hidden');
        hiddenInput.value = elem.userid;

        listElem.appendChild(text);
        listElem.appendChild(hiddenInput);

        listElem.addEventListener('click', e => {
            addSelectionToElement(e.target.textContent, e.target.querySelector('input').value);
            inputElem.value = '';
            closeLists();
        });
        list.appendChild(listElem);
    });
    return list;
};

const addSelectionToElement = (selection, userIdData) => {
    console.log(selection);
    console.log(userIdData);
    const resultContainer = document.querySelector('.new_room_selected_users');
    if (resultContainer) {
        const outerDiv = document.createElement('div');
        const spanElement = document.createElement('span');
        spanElement.innerText = selection;


        const buttonElement = document.createElement('button');
        buttonElement.classList.add('close');
        buttonElement.innerHTML = '&times;';

        buttonElement.addEventListener('click', e => {
            const clickedButton = e.target;
            resultContainer.removeChild(clickedButton.parentElement);
            e.preventDefault();
        });
        // here we want to set user id
        const hiddenData = document.createElement('input');
        hiddenData.setAttribute('type', 'hidden');
        hiddenData.value = userIdData;

        outerDiv.appendChild(spanElement);
        outerDiv.appendChild(buttonElement);
        outerDiv.appendChild(hiddenData);


        resultContainer.appendChild(outerDiv);
    }
};

const autoComplete = (inputElement) => {
    let listHandle;
    inputElement.addEventListener('input',
        e => {
            closeLists();
            const inputValue = e.target.value;
            if (!inputValue) {
                return false;
            }

            fetch(`/api/users/search?q=${inputValue}`)
                .then(res => res.json())
                .then(obj => {
                    listHandle = createAutoCompleteList(obj.data, inputElement);
                    e.target.parentNode.appendChild(listHandle);
                })
                .catch(err => console.error(err));
            return true;
        });

    inputElement.addEventListener('keydown',
        e => {
            const activeClassName = 'autocomplete-active';
            const activeClassNameQuery = '.' + activeClassName;
            if (!listHandle) {
                return;
            }
            const selectedElement = listHandle.querySelector(activeClassNameQuery);
            if (!selectedElement) {
                const firstDiv = listHandle.querySelector('div');
                if (firstDiv) {
                    firstDiv.classList.add(activeClassName);
                }
            } else {
                if (e.key === 'ArrowDown') {
                    selectedElement.classList.remove(activeClassName);
                    if (!selectedElement.nextElementSibling) {
                        listHandle.querySelector('div').classList.add(activeClassName);
                    } else {
                        selectedElement.nextElementSibling.classList.add(activeClassName);
                    }
                }

                if (e.key === 'ArrowUp') {
                    selectedElement.classList.remove(activeClassName);
                    if (!selectedElement.previousElementSibling) {
                        listHandle.querySelector('div').parentNode.lastChild.classList.add(activeClassName);
                    } else {
                        selectedElement.previousElementSibling.classList.add(activeClassName);
                    }
                }
            }
            if (e.key === 'Enter') {
                const recentSelectedElement = listHandle.querySelector(activeClassNameQuery);
                if (recentSelectedElement) {
                    recentSelectedElement.click();
                }
                e.preventDefault();
            }

        });

    document.addEventListener('click', e => closeLists());
};

const closeLists = () => { 
    document.querySelectorAll('.autocomplete-items').forEach(item => {
        item.parentNode.removeChild(item);
    });
};