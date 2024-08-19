async function onviewcart() {
    const mobileNumber = localStorage.getItem('mobileNumber');

    const userResponse = await fetch(`/api/User/mobile/${mobileNumber}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    if (!userResponse.ok) throw new Error(await userResponse.text());

    const user = await userResponse.json();
    console.log(user);
    const userId = user.userId;
    console.log('userid:'+userId);

    if (!userId) {
        alert('User is not logged in!');
    }

    viewCart(userId);
}

async function viewCart(userId) {
    try {
        let response = await fetch(`/api/Carts/user/${userId}/active`);
        if (!response.ok) {
            throw new Error('Failed to get active cart');
        }

        let cart = await response.json();
        let cartId = cart.cartId;
        console.log('cartid:' + cartId);

        let cartResponse = await fetch(`/api/CartItem`);
        if (!response.ok) {
            throw new Error('Failed to get cart items');
        }
        console.log(cartResponse);
        let cartItems = await cartResponse.json();
        console.log(cartItems);

        cartItems = cartItems.filter(item => item.cartId == cartId);
        console.log(cartItems);

        let cartList = document.getElementById('cart-list-body');
        cartList.innerHTML = '';

        if (cartItems.length == 0) {
            cartList.innerHTML = '<li>Your Cart is Empty</li>';
            return;
        }

        let grandTotal = 0;
        for (const item of cartItems) {
            let burgerresponse = await fetch(`/api/Burgers/${item.burgerId}`);
            let burger = await burgerresponse.json();
            console.log(burger);



            let total = burger.price * item.quantity;
            grandTotal += total;

            const listItem = document.createElement('tr');

            listItem.innerHTML = `
                <td>${burger.name}</td>
                <td>${burger.price}</td>
                <td>${total / burger.price}</td>
                <td>${total}</td>
                <td><button class="remove-button">Remove</button></td>
            `;

            listItem.querySelector('.remove-button').addEventListener('click', () => removeFromCart(item.cartItemId));

            cartList.appendChild(listItem);
    }

        let grandTotalElement = document.createElement('tr');
        grandTotalElement.innerHTML = `<tr><strong>Total Price:</strong></tr>
        <td></td>
        <td></td>
        <td><strong>₹${grandTotal}</strong></td>
        <td><button id="placeOrder">Place Order</button>`;
        grandTotalElement.querySelector('#placeOrder').addEventListener('click', () => onPlaceOrder());
        cartList.appendChild(grandTotalElement);

        
    }
    catch (error) {
        console.log('Error viewing cart:' + error);
        alert('Error viewing cart:' + error);
    }
}

async function removeFromCart(cartItemId) {

    console.log(cartItemId);
    try {
        let response = await fetch(`/api/CartItem/${cartItemId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Failed to remove item from cart');
        }
        alert('Item removed from cart');

        //for viewing the edited cart
        const mobileNumber = localStorage.getItem('mobileNumber');

        const userResponse = await fetch(`/api/User/mobile/${mobileNumber}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        if (!userResponse.ok) throw new Error(await userResponse.text());

        const user = await userResponse.json();
        console.log(user);
        const userId = user.userId;
        console.log(userId);

        viewCart(userId);
    }
    catch (error) {
        console.log('Error deleting item:' + error);
        alert('Error deleting item:' + error);
    }
}

async function onPlaceOrder() {
    const mobileNumber = localStorage.getItem('mobileNumber');

    const userResponse = await fetch(`/api/User/mobile/${mobileNumber}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    if (!userResponse.ok) throw new Error(await userResponse.text());

    const user = await userResponse.json();
    console.log(user);
    const userId = user.userId;
    console.log('userid:' + userId);

    if (!userId) {
        alert('User is not logged in!');
    }

    placeOrder(userId);
}

async function placeOrder(userId) {
    try {
        let response = await fetch(`/api/Carts/{userId}/order`);
        if (!response.ok) {
            throw new Error('Failed to place order');
        }
        alert('Congratulations order has been placed!');
    }
    catch (error) {
        console.log('Error placing order:' + error);
        alert('Error placing order:' + error);
    }
}
