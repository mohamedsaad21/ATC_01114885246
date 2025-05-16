document.addEventListener('DOMContentLoaded', () => {
    fetch('https://localhost:44390/api/event/')
        .then(res => {
            if (!res.ok) {
                return res.text().then(text => { throw new Error(text) })
            }
            return res.json()
        })
        .then(data => {
            const events = data.result
            const container = document.getElementById('eventsContainer')
            container.innerHTML = ''

            events.forEach(event => {
                const card = document.createElement('div')
                card.className = 'card mb-3 col-md-3'

                const imageUrl = 'https://localhost:44390' + event.imageUrl.replace(/\\/g, '/')

                card.innerHTML = `
                    <h3 class="card-header">${event.name}</h3>
                    <div class="card-body">
                        <h5 class="card-title">Category ID: ${event.categoryId}</h5>
                        <h6 class="card-subtitle text-muted">${new Date(event.date).toLocaleDateString()}</h6>
                    </div>
                    <img src="${imageUrl}" alt="${event.name}" class="card-img-top" style="height: 200px; object-fit: cover;">
                    <div class="card-body">
                        <p class="card-text">${event.description}</p>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li class="list-group-item">Venue: ${event.venue}</li>
                        <li class="list-group-item">Price: ${event.price}</li>
                    </ul>
                    <div class="card-footer text-muted">
                        <div>${new Date(event.date).toLocaleString()}</div>
                        <div><button type="button" class="btn btn-primary book-btn" data-event-id="${event.id}">Book Now</button></div>
                    </div>
                `
                container.appendChild(card)
            })

            document.querySelectorAll('.book-btn').forEach(button => {
                button.addEventListener('click', () => {
                    const eventId = button.getAttribute('data-event-id')
                    const token = localStorage.getItem('token')
                    const applicationUserId = localStorage.getItem('applicationUserId')

                    if (!token) {
                        alert('You must be logged in')
                        return
                    }

                    if (!applicationUserId) {
                        alert('User ID missing. Please login again.')
                        return
                    }

                    fetch('https://localhost:44390/api/booking', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + token
                        },
                        body: JSON.stringify({ eventId: eventId, applicationUserId: applicationUserId })
                    })
                        .then(res => {
                            if (!res.ok) {
                                return res.text().then(text => { throw new Error(text) })
                            }
                            return res.json()
                        })
                        .then(result => {
                            alert('Booking successful')
                        })
                        .catch(err => {
                            alert('Booking failed: ' + err.message)
                        })
                })
            })
        })
        .catch(err => {
            console.error('Error:', err.message)
        })
})
