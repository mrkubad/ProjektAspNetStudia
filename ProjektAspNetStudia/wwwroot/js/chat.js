const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start()
    .then(() => console.log('Connected to HUB!'))
    .catch(err => {
        console.log(err);
    });

let messagesDiv;

const sendMessage = (inputElement) => {
    const messageText = inputElement.value;
    if (messageText.length === 0) {
        return;
    }

    connection.invoke('SendMessage', messageText)
        .then(res => {
            inputElement.value = ''; // clear input
        })
        .catch(err => {
            return console.error(err);
        });
};

document.addEventListener('DOMContentLoaded', () => {
    messagesDiv = document.querySelector('.msg_history');
    scrollToElementBottom(messagesDiv);

    const messageInputElement = document.querySelector('.write_msg');

    document.querySelector(".msg_send_btn").addEventListener('click', e => {
        sendMessage(messageInputElement);
        e.preventDefault();
    });

    messageInputElement.addEventListener('keypress', e => {
        if (e.key === 'Enter') {
            sendMessage(messageInputElement);
            e.preventDefault();
        }
    });

    autoComplete(document.querySelector('#createRoomUserName'));

    document.querySelector('#createRoomSendData').addEventListener('click', e => {
        const selectedUsers = [];
        document.querySelector('#createRoomSelectedUsers').querySelectorAll('input').forEach(input => selectedUsers.push(input.value));

        const roomNameInput = document.querySelector('#createRoomRoomName');
        const roomName = roomNameInput.value;

        const url = '/api/room/create';

        const requestOptions = {
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                users: selectedUsers,
                name: roomName
            }),
            method: 'POST'
        };
        console.log(requestOptions.body);
        fetch(url, requestOptions)
            .then(res => {
                location.reload();
            })
            .catch(err => console.error(err));
    });



    document.querySelectorAll('.chat_list').forEach(item => {
        item.addEventListener('click',
            e => {
                if (item.classList.contains('active_chat')) {
                    return;
                }
                const chatId = item.dataset.chatid;
                if (item.classList.contains('chat_unread')) {
                    connection.invoke('MarkChatAsRead', chatId).catch(err => console.error(err));
                }
                const pathParts = window.location.pathname.split('/');
                if (pathParts.length !== 5) {
                    return;
                }

                pathParts[4] = chatId;

                const joinedPath = pathParts.join('/');
                console.log(joinedPath);

                window.location.pathname = joinedPath;
            });
    });
});

const createSentMessage = (text, strSentWhen) => {
    const outerDiv = document.createElement('div');
    outerDiv.className += 'outgoing_msg';

    const innerDiv = document.createElement('div');
    innerDiv.className += 'sent_msg';

    const message = document.createElement('p');
    message.innerText = text;

    const timeSent = document.createElement('span');
    timeSent.className += 'time_date';
    timeSent.innerText = strSentWhen;

    innerDiv.appendChild(message);
    innerDiv.appendChild(timeSent);

    outerDiv.appendChild(innerDiv);

    return outerDiv;
};

const createReceivedMessage = (messageText, sentWhen, senderName) => {
    const outerDiv = document.createElement('div');
    outerDiv.className += 'incoming_msg';

    const incomingMsgImg = document.createElement('div');
    incomingMsgImg.className += 'incoming_msg_img';

    const img = document.createElement('img');
    img.src = '/img/user-profile.png';
    img.alt = 'userPicture';

    const receivedMessage = document.createElement('div');
    receivedMessage.className += 'received_msg';

    const receivedMessageWithDate = document.createElement('div');
    receivedMessageWithDate.className += 'received_withd_msg';

    const message = document.createElement('p');
    message.innerText = messageText;

    const dateSent = document.createElement('span');
    dateSent.className += 'time_date';
    dateSent.innerText = sentWhen;

    const sentBy = document.createElement('span');
    sentBy.classList.add('sent_by');
    sentBy.innerText = senderName;

    receivedMessageWithDate.appendChild(sentBy);
    receivedMessageWithDate.appendChild(message);
    receivedMessageWithDate.appendChild(dateSent);

    receivedMessage.appendChild(receivedMessageWithDate);

    incomingMsgImg.appendChild(img);

    outerDiv.appendChild(incomingMsgImg);
    outerDiv.appendChild(receivedMessage);

    return outerDiv;
};

const refreshChatDateAndLastMessage = (chatElem, lastMessage, lastModificationDate, markAsNotRead) => {
 
    const chatId = chatElem.dataset.chatid;

    const parent = chatElem.parentElement;

    parent.removeChild(chatElem); // remove chat element

    const recentMessage = chatElem.querySelector('p');
    recentMessage.innerText = lastMessage;

    const recentModificationDate = chatElem.querySelector('.chat_date');
    recentModificationDate.innerText = lastModificationDate;

    if (markAsNotRead === true) {
        chatElem.classList.add('chat_unread');
        connection.invoke('MarkChatAsNotRead', chatId).catch(err => console.error(err));
    }

    parent.insertBefore(chatElem, parent.firstElementChild);
};

const scrollToElementBottom = (element) => {
    element.scrollTop = element.scrollHeight;
};


connection.on("ReceiveMessage",
    (messageObj, sentByMe) => {
        if (sentByMe === true) {
            const sentMessage = createSentMessage(messageObj.text, messageObj.sent);
            messagesDiv.appendChild(sentMessage);
           
        } 
        else {
            const receivedMessage = createReceivedMessage(messageObj.text, messageObj.sent, messageObj.userName);
            messagesDiv.appendChild(receivedMessage);
        }
        scrollToElementBottom(messagesDiv);
        refreshChatDateAndLastMessage(document.querySelector('.active_chat'), messageObj.text, messageObj.sentShort, false);
    });


connection.on('ReceiveNotification',
    (messageObj, chatId) => {
        console.log(`ChatId: ${chatId}`);
        document.querySelectorAll('.chat_list').forEach(elem => {

            if (elem.dataset.chatid != chatId) {
                return;
            }

            refreshChatDateAndLastMessage(elem, messageObj.text, messageObj.sentShort, true);
        }).catch(err => console.error(err));
    });