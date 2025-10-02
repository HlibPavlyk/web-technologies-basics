// SignalR Chat Client
"use strict";

// Global variables
let connection = null;
let currentUser = "";
let currentRoom = "";

// DOM elements
const loginForm = document.getElementById("loginForm");
const chatInterface = document.getElementById("chatInterface");
const userNameInput = document.getElementById("userName");
const roomNumberInput = document.getElementById("roomNumber");
const joinBtn = document.getElementById("joinBtn");
const leaveBtn = document.getElementById("leaveBtn");
const messageInput = document.getElementById("messageInput");
const sendBtn = document.getElementById("sendBtn");
const messagesList = document.getElementById("messagesList");
const messagesArea = document.getElementById("messagesArea");
const currentUserSpan = document.getElementById("currentUser");
const currentRoomSpan = document.getElementById("currentRoom");
const usersList = document.getElementById("usersList");
const usersCount = document.getElementById("usersCount");

// Initialize on page load
document.addEventListener("DOMContentLoaded", function () {
    setupEventListeners();
    setupSignalRConnection();
});

// Setup event listeners
function setupEventListeners() {
    // Join chat button
    joinBtn.addEventListener("click", joinChat);
    
    // Leave chat button
    leaveBtn.addEventListener("click", leaveChat);
    
    // Send message button
    sendBtn.addEventListener("click", sendMessage);
    
    // Enter key to send message
    messageInput.addEventListener("keypress", function (e) {
        if (e.key === "Enter") {
            sendMessage();
        }
    });
    
    // Enter key to join chat
    roomNumberInput.addEventListener("keypress", function (e) {
        if (e.key === "Enter") {
            joinChat();
        }
    });
    
    userNameInput.addEventListener("keypress", function (e) {
        if (e.key === "Enter") {
            joinChat();
        }
    });
}

// Setup SignalR connection
function setupSignalRConnection() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    // Handle receiving message
    connection.on("ReceiveMessage", function (userName, message, time) {
        addMessage(userName, message, time, false);
    });

    // Handle user joining
    connection.on("UserJoined", function (userName, time) {
        addSystemMessage(`${userName} joined the room`, time);
    });

    // Handle user leaving
    connection.on("UserLeft", function (userName, time) {
        addSystemMessage(`${userName} left the room`, time);
    });

    // Confirmation of joining room
    connection.on("JoinedRoom", function (roomNumber, userList) {
        showChatInterface();
        updateUsersList(userList || []);
    });

    // Handle user list updates
    connection.on("UserListUpdated", function (userList) {
        updateUsersList(userList);
    });

    // Handle join room failure
    connection.on("JoinRoomFailed", function (errorMessage) {
        showError(errorMessage);
        joinBtn.disabled = false;
        userNameInput.focus();
    });

    // Start connection
    connection.start().then(function () {
        console.log("SignalR connected successfully");
        joinBtn.disabled = false;
    }).catch(function (err) {
        console.error("SignalR connection error:", err);
        showError("Failed to connect to server. Please try again later.");
    });
}

// Join chat
function joinChat() {
    const userName = userNameInput.value.trim();
    const roomNumber = roomNumberInput.value.trim();

    // Validation
    if (!userName) {
        showError("Please enter your name");
        userNameInput.focus();
        return;
    }

    if (!roomNumber) {
        showError("Please enter room number");
        roomNumberInput.focus();
        return;
    }

    if (userName.length > 50) {
        showError("Name cannot be longer than 50 characters");
        return;
    }

    if (roomNumber.length > 20) {
        showError("Room number cannot be longer than 20 characters");
        return;
    }

    // Disable join button to prevent multiple requests
    joinBtn.disabled = true;

    // Save current data
    currentUser = userName;
    currentRoom = roomNumber;

    // Send join request
    connection.invoke("JoinRoom", userName, roomNumber).catch(function (err) {
        console.error("Error joining room:", err);
        showError("Failed to join room");
        joinBtn.disabled = false;
    });
}

// Leave chat
function leaveChat() {
    if (connection && currentUser && currentRoom) {
        joinBtn.disabled = false;
        connection.invoke("LeaveRoom", currentUser, currentRoom).catch(function (err) {
            console.error("Error leaving room:", err);
        });
    }

    // Return to login form
    showLoginForm();
    clearMessages();
    currentUser = "";
    currentRoom = "";
}

// Send message
function sendMessage() {
    const message = messageInput.value.trim();

    if (!message) {
        messageInput.focus();
        return;
    }

    if (message.length > 500) {
        showError("Message cannot be longer than 500 characters");
        return;
    }

    // Send message via SignalR
    connection.invoke("SendMessage", currentUser, message, currentRoom).catch(function (err) {
        console.error("Error sending message:", err);
        showError("Failed to send message");
    });

    // Clear input field
    messageInput.value = "";
    messageInput.focus();
}

// Show chat interface
function showChatInterface() {
    loginForm.style.display = "none";
    chatInterface.style.display = "flex";
    currentUserSpan.textContent = currentUser;
    currentRoomSpan.textContent = currentRoom;
    messageInput.focus();
}

// Show login form
function showLoginForm() {
    chatInterface.style.display = "none";
    loginForm.style.display = "flex";
    userNameInput.focus();
}

// Add message to chat
function addMessage(userName, message, time, isOwnMessage = false) {
    const messageDiv = document.createElement("div");
    messageDiv.className = `mb-2 ${isOwnMessage ? 'text-end' : ''}`;
    
    const messageClass = isOwnMessage ? 'bg-primary text-white' : 'bg-light';
    const alignment = isOwnMessage ? 'ms-auto' : '';
    
    messageDiv.innerHTML = `
        <div class="d-inline-block p-2 rounded ${messageClass} ${alignment}" style="max-width: 70%;">
            <div class="fw-bold small">${escapeHtml(userName)}</div>
            <div>${escapeHtml(message)}</div>
            <div class="small opacity-75">${time}</div>
        </div>
    `;

    messagesList.appendChild(messageDiv);
    scrollToBottom();
}

// Add system message
function addSystemMessage(message, time) {
    const messageDiv = document.createElement("div");
    messageDiv.className = "mb-2 text-center";
    
    messageDiv.innerHTML = `
        <div class="d-inline-block p-2 rounded bg-secondary text-white small">
            <div>${escapeHtml(message)}</div>
            <div class="small opacity-75">${time}</div>
        </div>
    `;

    messagesList.appendChild(messageDiv);
    scrollToBottom();
}

// Clear messages
function clearMessages() {
    messagesList.innerHTML = "";
}

// Scroll to bottom
function scrollToBottom() {
    messagesArea.scrollTop = messagesArea.scrollHeight;
}

// Show error
function showError(message) {
    // Simple alert for demonstration, can be replaced with a nicer toast
    alert(message);
}

// Escape HTML
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Update users list
function updateUsersList(users) {
    usersList.innerHTML = '';
    usersCount.textContent = users.length;
    
    users.forEach(user => {
        const userItem = createUserItem(user);
        usersList.appendChild(userItem);
    });
}

// Create user item element
function createUserItem(userName) {
    const userItem = document.createElement('div');
    userItem.className = 'user-item';
    
    const avatar = document.createElement('div');
    avatar.className = 'user-avatar';
    avatar.textContent = userName.charAt(0).toUpperCase();
    
    const name = document.createElement('div');
    name.className = 'user-name';
    name.textContent = userName;
    
    const status = document.createElement('div');
    status.className = 'user-status';
    
    userItem.appendChild(avatar);
    userItem.appendChild(name);
    userItem.appendChild(status);
    
    return userItem;
}
