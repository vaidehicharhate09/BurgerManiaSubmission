console.log('test');
function formValidation() {
    const mobileInput = document.getElementById('mobile').value;
    const nameInput = document.getElementById('name').value;

    const mobilePattern = /^\d{10}$/;
    if (!mobilePattern.test(mobileInput)) {
        alert("Please enter a valid 10-digit mobile number.");
        return false; 
    }

    if (!nameInput.trim()) {
        alert("Name cannot be empty.");
        return false; 
    }

    
    checkAndCreateUser(mobileInput, nameInput);

    return false; 
}
//async function loginUser(mobileNumber, name) {
//    try {
//        const response = await fetch(`/api/User/login`, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json'
//            },
//            body: JSON.stringify({ mobileNumber, name })
//        });

//        if (!response.ok) throw new Error(await response.text());

//        const user = await response.json();
//        localStorage.setItem('userId', user.userId);
//        window.location.href = "/home";
//    } catch (error) {
//        console.error('Login failed:', error);
//        alert('Login failed: ' + error.message);
//    }
//}
async function checkAndCreateUser(mobile, name) {
    try {
        const response = await fetch(`api/User/mobile/${mobile}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

    
        const result = await response.json();

        if (response.ok) {
            alert("User already exists. No new entry created.");
            /*localStorage.setItem('userId', result.userId);*/
            /*alert(result.userId);*/
            localStorage.setItem('mobileNumber', mobile);
            window.location.href = './home.html'; 
        }
        if (!response.ok) {
            const createResponse = await fetch('api/User', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    "mobileNumber": mobile,
                    "name": name

                })
            });
            console.log(createResponse);
            if (createResponse.ok) {
                alert("User created successfully!");
                window.location.href = './home.html';
            } else {
                if (createResponse.status === 409) {
                    alert("Conflict: User already exists.");
                } else {
                    alert("Error creating user. Please try again.");
                }
            }
        }
    }
    catch (error) {
        console.error("Error checking and creating user:", error);
        alert("An error occurred while processing the user. Please try again.");
    }
}