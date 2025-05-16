document.getElementById('loginForm').addEventListener('submit', function (e) {
    e.preventDefault()
    const email = document.getElementById('email').value
    const password = document.getElementById('password').value

    fetch('https://localhost:44390/api/auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password })
    })
        .then(res => {
            if (!res.ok) {
                return res.text().then(text => { throw new Error(text) })
            }
            return res.json()
        })
        .then(data => {
            // Save token and applicationUserId from response result
            localStorage.setItem('token', data.result.token)
            localStorage.setItem('applicationUserId', data.result.applicationUserId)
            document.getElementById('message').textContent = 'Logged in'
        })
        .catch(err => {
            document.getElementById('message').textContent = err.message
        })
})
