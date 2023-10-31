$('.product-buy').click(function () {
    // Find the closest '.product-bottom' element relative to the clicked icon and add the class.
    $(this).closest('.product-bottom').addClass("product-clicked");

    // Your JavaScript to call the Add action
    let productId = $(this).data("id"); // assuming that you have data-id attribute set on your .product-buy element
    $.ajax({
        url: `/Cart/Add?id=${productId}`,
        type: 'POST',
        success: function (response) {
            if (response.success) {

                console.log('Product added');
                const cartItem = response.cartItem;

				console.log(response);
				console.log(cartItem);

				const newProductHtml = generateProductHtml(cartItem);
                // Append newProductHtml to the cart or any other container where you display products
				$('#small-cart-container').append(newProductHtml);
				$('#small-cart-sum').text(response.cartSum);
				$('.cart-sum').text(response.cartSum);
			} else {
				console.log('Failed to add product:', response.message);
			}
        },
        error: function (err) {
            console.log('Error:', err);
        }
    });
});

$('.cart-item-increase').click(function () {
	let productId = $(this).data("id"); 
	$.ajax({
		url: `/Cart/Increase?id=${productId}`,
		type: 'POST',
		success: function (response) {
			if (response.success) {

				console.log('Product Increased');
				const cartItem = response.cartItem;

				console.log(response);
				console.log(cartItem);
				$(`.cart-item-quantity[data-id='${ productId }']`).text(cartItem.quantity);
				$('.cart-sum').text(response.cartSum);
				$('#small-cart-sum').text(response.cartSum);
			} else {
				console.log('Failed to add product:', response.message);
			}
		},
		error: function (err) {
			console.log('Error:', err);
		}
	});
});
$('.cart-item-decrease').click(function () {
	let productId = $(this).data("id"); 
	$.ajax({
		url: `/Cart/Decrease?id=${productId}`,
		type: 'POST',
		success: function (response) {
			if (response.success) {
				if (response.remove) {
					$(`.product-bottom[data-id='${ productId }']`).removeClass("product-clicked");

					console.log('Product removed');
					const cartItem = response.cartItem;

					console.log(response);
					console.log(cartItem);


					// Append newProductHtml to the cart or any other container where you display products
					$(`.small-cart-item[data-id='${productId}']`).remove();
				$('.cart-sum').text(response.cartSum);
					$('#small-cart-sum').text(response.cartSum);
				}
				else {
					console.log('Product Decreased');
					const cartItem = response.cartItem;

					console.log(response);
					console.log(cartItem);
					$(`.cart-item-quantity[data-id='${productId}']`).text(cartItem.quantity);
				$('.cart-sum').text(response.cartSum);
					$('#small-cart-sum').text(response.cartSum);
				}
			} else {
				console.log('Failed to decrease product:', response.message);
			}
		},
		error: function (err) {
			console.log('Error:', err);
		}
	});
});


$('.product-remove').click(function () {
    // Find the closest '.product-bottom' element relative to the clicked remove icon and remove the class.
    $(this).closest('.product-bottom').removeClass("product-clicked");

    // Your JavaScript to call the Remove action
    let productId = $(this).data("id"); // assuming that you have data-id attribute set on your .product-remove element
    $.ajax({
        url: `/Cart/Remove?id=${productId}`,
        type: 'POST',
        success: function (response) {
            // Handle the response
			console.log('Product removed');

			if (response.success) {

				console.log('Product removed');
				const cartItem = response.cartItem;

				console.log(response);
				console.log(cartItem);
				$(`.product-bottom[data-id='${productId}']`).removeClass("product-clicked");

				
				// Append newProductHtml to the cart or any other container where you display products
				$(`.small-cart-item[data-id='${productId}']`).remove();
				$('.cart-sum').text(response.cartSum);
				$('#small-cart-sum').text(response.cartSum);
				$(`.small-cart-submenu`).removeClass("show");
			} else {
				console.log('Failed to remove product:', response.message);
			}


        },
        error: function (err) {
            console.log('Error:', err);
        }
    });
});


function generateProductHtml(cartItem) {
	return `
<div class="border border-dashed border-gray-300 rounded px-7 py-3 mb-6 small-cart-item" data-id="${cartItem.productId}">
	<div class="d-flex flex-stack mb-3">
		<div class="me-3">
			<img src="/media/products/${cartItem.image}" class="w-50px ms-n1 me-1" alt="" />
			<p class="text-gray-800 text-hover-primary fw-bold">${cartItem.productName}</p>
		</div>
		<div class="m-0">
			<button class="btn btn-icon btn-color-gray-400 btn-active-color-primary justify-content-end"
			data-kt-menu-trigger="click"
			data-kt-menu-placement="bottom-end"
			data-kt-menu-overflow="true">
				<i class="ki-duotone ki-dots-square fs-1"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span></i>
			</button>
			<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-200px small-cart-submenu" data-id="${cartItem.productId}" data-kt-menu="true">
				<div class="menu-item px-3">
					<div class="menu-content fs-6 text-dark fw-bold px-3 py-4">Quick Actions</div>
				</div>
				<div class="separator mb-3 opacity-75"></div>
				<div class="menu-item px-3">
					<p class="menu-link px-3 cart-item-increase" data-id="${cartItem.productId}">
						Increase
					</p>
				</div>
				<div class="menu-item px-3">
					<p class="menu-link px-3 cart-item-decrease" data-id="${cartItem.productId}">
						Decrease
					</p>
				</div>


				<div class="separator mt-3 opacity-75"></div>
				<div class="menu-item px-3">
					<div class="menu-content px-3 py-3">
						<p class="btn btn-danger  btn-sm px-4 cart-item-remove product-remove" data-id="${cartItem.productId}">
							Remove
						</p>
					</div>
				</div>
			</div>
		</div>
	</div>
	<div class="d-flex flex-stack">
		<span class="text-gray-400 fw-bold">
			Price:
			<p  class="text-gray-800 text-hover-primary fw-bold">
				${cartItem.price}
			</p>
		</span>
		<span class="badge badge-light-primary cart-item-quantity" data-id="${cartItem.productId}">${cartItem.quantity}</span>
	</div>
</div>
`;
}

/*
<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-200px show" data-kt-menu="true" data-popper-placement="bottom-end" style="z-index: 107; position: absolute; inset: 0px 0px auto auto; margin: 0px; transform: translate3d(-776.154px, 365.385px, 0px);">
	<div class="menu-item px-3">
		<div class="menu-content fs-6 text-dark fw-bold px-3 py-4">Quick Actions</div>
	</div>
	<div class="separator mb-3 opacity-75"></div>
	<div class="menu-item px-3">
		<p class="menu-link px-3 small-cart-increase" data-id="1">
			Increase
		</p>
	</div>
	<div class="menu-item px-3">
		<p class="menu-link px-3 small-cart-decrease" data-id="1">
			Decrease
		</p>
	</div>
										

	<div class="separator mt-3 opacity-75"></div>
	<div class="menu-item px-3">
		<div class="menu-content px-3 py-3">
			<p class="btn btn-danger  btn-sm px-4 small-cart-remove product-remove" data-id="1">
				Remove
			</p>
		</div>
	</div>
</div>

*/

