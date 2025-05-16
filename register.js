document.getElementById('registerForm').addEventListener('submit', function (e) {
    e.preventDefault()
   const firstname = document.getElementById('firstname').value;
   const lastname = document.getElementById('lastname').value;
   const username = document.getElementById('username').value;
   const email = document.getElementById('email').value
   const password = document.getElementById('password').value

    fetch('https://localhost:44390/api/auth/register', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
        },
        body: JSON.stringify({ firstname, lastname, username, email, password })
  })
    .then(res => {
      if (!res.ok) {
        return res.text().then(text => { throw new Error(text) })
      }
      return res.json()
    })
        .then(data => {
            localStorage.setItem('token', data.token)
            document.getElementById('message').textContent = 'registered'
        })
    .catch(err => {
      document.getElementById('message').textContent = err.message
    })
})
