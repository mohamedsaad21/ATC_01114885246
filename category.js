document.addEventListener('DOMContentLoaded', () => {
    const select = document.querySelector('.form-floating select.form-select')

    if (!select) return

    fetch('https://localhost:44390/api/category')
        .then(res => {
            if (!res.ok) throw new Error('Failed to load categories')
            return res.json()
        })
        .then(data => {
            const categories = data.result
            select.innerHTML = '<option disabled selected>--Select Category--</option>'
            categories.forEach(cat => {
                const option = document.createElement('option')
                option.value = cat.id
                option.textContent = cat.name
                select.appendChild(option)
            })
        })
        .catch(err => {
            console.error('Error loading categories:', err)
        })
})