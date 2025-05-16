document.addEventListener('DOMContentLoaded', async () => {
    const select = document.querySelector('select.form-select')

    try {
        const res = await fetch('https://localhost:44390/api/categories')
        const data = await res.json()
        const categories = data.result

        categories.forEach(cat => {
            const option = document.createElement('option')
            option.value = cat.id
            option.textContent = cat.name
            select.appendChild(option)
        })
    } catch (err) {
        console.error('Error loading categories:', err)
    }
})

document.querySelector('form').addEventListener('submit', async e => {
    e.preventDefault()

    const form = e.target
    const formData = new FormData()

    formData.append('name', form.querySelector('input[name="name"]').value)
    formData.append('description', form.querySelector('textarea[name="description"]').value)
    formData.append('date', form.querySelector('input[name="date"]').value)
    formData.append('venue', form.querySelector('input[name="venue"]').value)
    formData.append('price', form.querySelector('input[type="number"]').value)
    formData.append('categoryId', form.querySelector('select.form-select').value)

    const fileInput = form.querySelector('input[type="file"]')
    if (fileInput.files[0]) {
        formData.append('file', fileInput.files[0])
    }

    try {
        const res = await fetch('https://localhost:44390/api/events', {
            method: 'POST',
            body: formData
        })

        if (!res.ok) {
            const error = await res.json()
            console.error('Create failed:', error)
            alert('Error posting event')
            return
        }

        alert('Event created')
        form.reset()
        document.getElementById('bookPicture').src = ''
    } catch (err) {
        console.error('Post error:', err)
        alert('Network error')
    }
})
