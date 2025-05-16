document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token')
    const loginNav = document.getElementById('login-nav')
    const registerNav = document.getElementById('register-nav')
    const userNav = document.getElementById('user-nav')
    const usernameDisplay = document.getElementById('username-display')
    const logoutBtn = document.getElementById('logout-btn')

    function parseJwt(token) {
        if (!token) return null
        const base64Url = token.split('.')[1]
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        )
        return JSON.parse(jsonPayload)
    }

    if (token) {
        const payload = parseJwt(token)
        if (payload && payload.sub) {
            usernameDisplay.textContent = payload.sub

            loginNav.classList.add('d-none')
            registerNav.classList.add('d-none')
            userNav.classList.remove('d-none')
        }
    }

    logoutBtn.addEventListener('click', () => {
        localStorage.removeItem('token')
        location.reload()
    })
})
