 async function addToCart(burgerName) {
    const mobileNumber = localStorage.getItem('mobileNumber');

    if (!mobileNumber) {
        alert('User is not logged in. Please log in first.');
        return;
    }

    // Get the burger option and quantity from the UI
    const optionElement = document.querySelector(`input[name="${burgerName.toLowerCase().replace(/\s+/g, '')}-option"]:checked`);
    if (!optionElement) {
        alert("Please select an option (Veg, Non-Veg, or Egg).");
        return;
    }

    const option = optionElement.value;
    const quantity = document.querySelector(`input[name="${burgerName.toLowerCase().replace(/\s+/g, '')}-quantity"]`).value;

    if (!quantity || quantity <= 0) {
        alert("Please enter a valid quantity.");
        return;
    }

     try {
         // Step 1: Get the user ID by mobile number
         const userResponse = await fetch(`/api/User/mobile/${mobileNumber}`, {
             method: 'GET',
             headers: {
                 'Content-Type': 'application/json'
             }
         })
         if (!userResponse.ok) throw new error(await userResponse.text());

         const user = await userResponse.json();
         console.log(user);
         const userId = user.userId;
         console.log(userId);

         //Step 2: Get or create the user's active cart
         let cartResponse = await fetch(`/api/Carts/user/${userId}/active`);
         let cart;

         if (cartResponse.status === 404) {
             // If no active cart exists, create a new one
             cartResponse = await fetch(`/api/Cart/user/${userId}/create`, { method: 'POST' });
             if (!cartResponse.ok) throw new Error(await cartResponse.text());
             cart = await cartResponse.json();
         } else if (!cartResponse.ok) {
             throw new Error(await cartResponse.text());
         } else {
             cart = await cartResponse.json();
         }

         const cartId = cart.cartId;
         console.log(cart.cartId);

         // Step 3: Get the burger ID based on name and type
         const burgerResponse = await fetch(`/api/Burgers/getByNameAndType?name=${burgerName}&type=${option}`);
         if (!burgerResponse.ok) throw new Error(await burgerResponse.text());

         const burger = await burgerResponse.json();
         const burgerId = burger.burgerId;
         console.log(burgerId);

         //Step 4: Add the burger to the cart
         const cartItem = {
             userId: userId,
             cartId: cartId,
             burgerId: burgerId,
             quantity: parseInt(quantity)
         };

         const cartItemResponse = await fetch(`/api/CartItem`, {
             method: 'POST',
             headers: {
                 'Content-Type': 'application/json'
             },
             body: JSON.stringify(cartItem)
         });

         if (!cartItemResponse.ok) throw new Error(await cartItemResponse.text());

         const addedCartItem = await cartItemResponse.json();

         alert("Item added to cart successfully!");

             
    } catch (error) {
        console.error('Error adding item to cart:', error);
        alert('Error adding item to cart: ' + error.message);
    }
}