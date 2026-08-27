document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('saveTaskBtn')?.addEventListener('click', saveTask);
    document.getElementById('updateTaskBtn')?.addEventListener('click', saveTaskChanges);
    initializeDragAndDrop();

    document.addEventListener('click', event => {
        const editButton = event.target.closest('.edit-btn');
        if (editButton) {
            openEditModal(editButton.closest('.task-card'));
            return;
        }

        const deleteButton = event.target.closest('.delete-btn');
        if (deleteButton) {
            const taskCard = deleteButton.closest('.task-card');
            if (confirm('Czy na pewno usunąć to zadanie?')) {
                deleteTask(taskCard.dataset.taskId);
            }
        }
    });
});

function openEditModal(taskCard) {
    document.getElementById('editTaskId').value = taskCard.dataset.taskId;
    document.getElementById('editTaskTitle').value = taskCard.dataset.taskTitle || '';
    document.getElementById('editTaskDescription').value = taskCard.dataset.taskDescription || '';
    document.getElementById('editTaskPriority').value = taskCard.dataset.taskPriority || '1';
    document.getElementById('editTaskAssignee').value = taskCard.dataset.taskAssignee || '';
    const timeSpentMinutes = Number(taskCard.dataset.taskTimeSpentMinutes || 0);
    document.getElementById('editTaskTimeHours').value = Math.floor(timeSpentMinutes / 60);
    document.getElementById('editTaskTimeMinutes').value = timeSpentMinutes % 60;
    document.getElementById('editTaskDueDate').value = taskCard.dataset.taskDueDate || '';
    bootstrap.Modal.getOrCreateInstance(document.getElementById('editTaskModal')).show();
}

function initializeDragAndDrop() {
    let draggedCard = null;
    let sourceList = null;

    document.querySelectorAll('.task-card').forEach(card => {
        card.addEventListener('dragstart', event => {
            if (event.target.closest('button, a, input, textarea, select, label')) {
                event.preventDefault();
                return;
            }

            draggedCard = card;
            sourceList = card.closest('.kanban-list');
            card.classList.add('dragging');
            event.dataTransfer.effectAllowed = 'move';
            event.dataTransfer.setData('text/plain', card.dataset.taskId);
        });

        card.addEventListener('dragend', () => {
            card.classList.remove('dragging');
            document.querySelectorAll('.kanban-list').forEach(list => list.classList.remove('drag-over'));
            draggedCard = null;
            sourceList = null;
        });
    });

    document.querySelectorAll('.kanban-list').forEach(list => {
        list.addEventListener('dragenter', event => {
            event.preventDefault();
            list.classList.add('drag-over');
        });

        list.addEventListener('dragover', event => {
            event.preventDefault();
            event.dataTransfer.dropEffect = 'move';
            list.classList.add('drag-over');
        });

        list.addEventListener('dragleave', event => {
            if (!list.contains(event.relatedTarget)) {
                list.classList.remove('drag-over');
            }
        });

        list.addEventListener('drop', async event => {
            event.preventDefault();
            list.classList.remove('drag-over');

            const taskId = event.dataTransfer.getData('text/plain');
            const card = draggedCard || document.querySelector(`.task-card[data-task-id="${CSS.escape(taskId)}"]`);
            const previousList = sourceList || card?.closest('.kanban-list');

            if (!card || !previousList || list === previousList) return;

            list.appendChild(card);

            try {
                await sendJson('/Kanban/UpdateStatus', {
                    id: Number(card.dataset.taskId),
                    status: Number(list.dataset.status)
                });
            } catch (error) {
                previousList.appendChild(card);
                console.error(error);
                alert(`Błąd: ${error.message}`);
            }
        });
    });
}

async function sendJson(url, data) {
    const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });

    const result = await response.json();
    if (!response.ok || !result.success) {
        throw new Error(result.message || 'Nie udało się wykonać operacji.');
    }

    return result;
}

async function saveTask() {
    const title = document.getElementById('taskTitle').value.trim();
    const description = document.getElementById('taskDescription').value.trim();
    const priority = Number(document.getElementById('taskPriority').value);
    const assignee = document.getElementById('taskAssignee').value.trim();
    const timeSpentMinutes = readTimeSpentMinutes('taskTimeHours', 'taskTimeMinutes');
    const dueDate = document.getElementById('taskDueDate').value || null;

    if (timeSpentMinutes === null) return;

    if (!title) {
        alert('Tytuł jest wymagany.');
        return;
    }

    try {
        await sendJson('/Kanban/Create', { title, description, priority, assignee, timeSpentMinutes, dueDate, status: 0 });
        bootstrap.Modal.getInstance(document.getElementById('addTaskModal'))?.hide();
        document.getElementById('addTaskForm').reset();
        location.reload();
    } catch (error) {
        console.error(error);
        alert(`Błąd: ${error.message}`);
    }
}

async function saveTaskChanges() {
    const id = Number(document.getElementById('editTaskId').value);
    const title = document.getElementById('editTaskTitle').value.trim();
    const description = document.getElementById('editTaskDescription').value.trim();
    const priority = Number(document.getElementById('editTaskPriority').value);
    const assignee = document.getElementById('editTaskAssignee').value.trim();
    const timeSpentMinutes = readTimeSpentMinutes('editTaskTimeHours', 'editTaskTimeMinutes');
    const dueDate = document.getElementById('editTaskDueDate').value || null;

    if (timeSpentMinutes === null) return;

    if (!title) {
        alert('Tytuł jest wymagany.');
        return;
    }

    try {
        await sendJson('/Kanban/Update', { id, title, description, priority, assignee, timeSpentMinutes, dueDate });
        bootstrap.Modal.getInstance(document.getElementById('editTaskModal'))?.hide();
        location.reload();
    } catch (error) {
        console.error(error);
        alert(`Błąd: ${error.message}`);
    }
}

function readTimeSpentMinutes(hoursId, minutesId) {
    const hours = Number(document.getElementById(hoursId).value || 0);
    const minutes = Number(document.getElementById(minutesId).value || 0);

    if (!Number.isInteger(hours) || hours < 0 || !Number.isInteger(minutes) || minutes < 0 || minutes > 59) {
        alert('Podaj poprawny czas pracy. Minuty muszą mieć wartość od 0 do 59.');
        return null;
    }

    return hours * 60 + minutes;
}

async function deleteTask(taskId) {
    try {
        await sendJson('/Kanban/Delete', { id: Number(taskId) });
        location.reload();
    } catch (error) {
        console.error(error);
        alert(`Błąd: ${error.message}`);
    }
}
