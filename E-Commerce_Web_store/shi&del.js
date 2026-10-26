
const shippingModal = new bootstrap.Modal(
    document.getElementById("shippingModal")
);

// DEMO DATA:
// Use null when the customer has no order.
// Replace this sample object to test the placed-order case.
const order = {
    id: "HB1024",
    status: "Processing",
    payment: "Paid",
    date: "10 October 2026",
    address: "Chennai, Tamil Nadu - 600001",
    estimatedDelivery: "14–17 October 2026"
};

// To test the no-order case, change the object above to:
// const order = null;

function showInfo(type) {
    const title = document.getElementById("modalTitle");
    const body = document.getElementById("modalBody");

    if (type === "order") {
        title.textContent = "Order Processing";

        if (!order) {
            body.innerHTML = `
                <h6>No orders yet!</h6>
                <p>You haven't placed an order yet.
                Explore our herbal beauty products and find
                something you love.</p>
                <a href="products.html" class="btn btn-herbal">
                    Shop Now
                </a>
            `;
        } else {
            body.innerHTML = `
                <span class="status-badge">
                    ${order.status}
                </span>

                <h6>Order ID: ${order.id}</h6>
                <p>Order Date: ${order.date}</p>
                <p>Payment Status: ${order.payment}</p>

                <hr>

                <h6>Order Progress</h6>

                <div class="progress-step completed">
                    ✓ Order Placed
                </div>

                <div class="progress-step ${
                    ["Processing", "Shipped", "Delivered"]
                    .includes(order.status) ? "completed" : ""
                }">
                    ${order.status === "Processing" ? "●" : "✓"}
                    Processing
                </div>

                <div class="progress-step ${
                    ["Shipped", "Delivered"]
                    .includes(order.status) ? "completed" : ""
                }">
                    ${["Shipped", "Delivered"]
                    .includes(order.status) ? "✓" : "○"}
                    Shipped
                </div>

                <div class="progress-step ${
                    order.status === "Delivered" ? "completed" : ""
                }">
                    ${order.status === "Delivered" ? "✓" : "○"}
                    Delivered
                </div>
            `;
        }
    }

    else if (type === "delivery") {
        title.textContent = "Delivery Times";

        if (!order) {
            body.innerHTML = `
                <h6>When will my order arrive?</h6>
                <p>Delivery time depends on your PIN code
                and the availability of your products.</p>
                <p>Enter your delivery PIN code during
                checkout to see the estimated delivery date.</p>
            `;
        } else {
            body.innerHTML = `
                <span class="status-badge">
                    ${order.status}
                </span>

                <h6>Estimated Delivery</h6>
                <p>${order.estimatedDelivery}</p>

                <h6>Delivery Address</h6>
                <p>${order.address}</p>

                <p>Delivery dates are estimates and may change
                due to courier delays.</p>
            `;
        }
    }

    else if (type === "location") {
        title.textContent = "Delivery Location";

        body.innerHTML = `
            <h6>Is delivery available in your area?</h6>
            <p>Enter your 6-digit PIN code to check
            sample delivery availability.</p>

            <form onsubmit="checkPin(event)">
                <input
                    type="text"
                    id="pinCode"
                    placeholder="Enter 6-digit PIN code"
                    maxlength="6"
                    inputmode="numeric"
                    pattern="[0-9]{6}"
                    required
                >

                <button type="submit" class="btn btn-herbal">
                    Check Availability
                </button>
            </form>

            <p id="pinResult" class="mt-3"></p>

            ${order ? `
                <hr>
                <h6>Current Order Address</h6>
                <p>${order.address}</p>
            ` : ""}
        `;
    }

    shippingModal.show();
}

function checkPin(event) {
    event.preventDefault();

    const pin = document.getElementById("pinCode").value.trim();
    const result = document.getElementById("pinResult");

    if (!/^[0-9]{6}$/.test(pin)) {
        result.textContent = "Please enter a valid 6-digit PIN code.";
        return;
    }

    // DEMO ONLY: This does not check real courier availability.
    result.textContent =
        "PIN code entered successfully! Actual delivery availability " +
        "will be confirmed when connected to the delivery service.";
}
