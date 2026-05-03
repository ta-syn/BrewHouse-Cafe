// Update cart count badge
function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(r => r.json())
        .then(data => {
            const badge = document.getElementById('cart-count');
            if (badge) {
                badge.textContent = data.count;
                if (data.count > 0) {
                    badge.classList.remove('d-none');
                    badge.classList.add('animate__animated', 'animate__bounceIn');
                } else {
                    badge.classList.add('d-none');
                }
            }
        })
        .catch(() => {});
}

// Add item to cart
// Add item to cart with instant feedback
function addToCart(itemId) {
    // Show a small loading effect on the button if possible
    const btn = event?.currentTarget;
    if (btn) btn.classList.add('opacity-50');

    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { itemId: itemId, quantity: 1 },
        success: function(data) {
            if (data.success) {
                const badge = $('#cart-count');
                if (badge.length) {
                    badge.text(data.cartCount).removeClass('d-none');
                    
                    // Instant animation trigger
                    badge.removeClass('animate__animated animate__bounceIn');
                    void badge[0].offsetWidth; // force reflow
                    badge.addClass('animate__animated animate__bounceIn');
                    
                    // Animate cart icon
                    const icon = badge.parent().find('.fa-shopping-cart');
                    icon.removeClass('animate__animated animate__rubberBand');
                    void icon[0].offsetWidth;
                    icon.addClass('animate__animated animate__rubberBand');
                }
                
                showToast('Added to your basket!', 'success');
            } else {
                showToast(data.message || 'Error adding item', 'error');
            }
        },
        error: function() {
            showToast('Network error. Please try again.', 'error');
        },
        complete: function() {
            if (btn) btn.classList.remove('opacity-50');
        }
    });
}

// Global confirm delete with SweetAlert2
function confirmDelete(message) {
    return Swal.fire({
        title: 'Are you sure?',
        text: message || "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#2C1810',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    });
}

// Toast notification (fallback)
function showToast(message, type = 'success') {
    Swal.fire({
        icon: type === 'danger' ? 'error' : type,
        text: message,
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
}

document.addEventListener('DOMContentLoaded', function() {
    // Mobile sidebar toggle
    const toggleBtn = document.getElementById('sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', () => sidebar.classList.toggle('open'));
    }

    // Update cart count on load
    if (document.getElementById('cart-count')) updateCartCount();

    // Auto-init tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
});

// Global Toggle Password visibility
function togglePassword(inputId, iconEl) {
    const input = document.getElementById(inputId);
    const icon = iconEl.querySelector('i');
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

// Show message for existing members
function showAlreadyMemberMsg() {
    Swal.fire({
        title: 'Already a Member!',
        text: 'You are already a valued member of our coffee community. Thank you for your continued support! ☕✨',
        icon: 'info',
        confirmButtonColor: '#2C1810',
        confirmButtonText: 'Great!',
        showClass: {
            popup: 'animate__animated animate__fadeInDown'
        },
        hideClass: {
            popup: 'animate__animated animate__fadeOutUp'
        }
    });
}
