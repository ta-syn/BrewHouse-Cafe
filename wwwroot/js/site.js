// Update cart count badge
function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(r => r.json())
        .then(data => {
            const badge = document.getElementById('cart-count');
            const badgeMobile = document.getElementById('cart-count-mobile');
            
            [badge, badgeMobile].forEach(el => {
                if (el) {
                    el.textContent = data.count;
                    if (data.count > 0) {
                        el.classList.remove('d-none');
                        el.classList.add('animate__animated', 'animate__bounceIn');
                    } else {
                        el.classList.add('d-none');
                    }
                }
            });
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
                const badgeMobile = $('#cart-count-mobile');
                
                [badge, badgeMobile].forEach(el => {
                    if (el.length) {
                        el.text(data.cartCount).removeClass('d-none');
                        
                        // Instant animation trigger
                        el.removeClass('animate__animated animate__bounceIn');
                        void el[0].offsetWidth; // force reflow
                        el.addClass('animate__animated animate__bounceIn');
                        
                        // Animate cart icon
                        const icon = el.parent().find('.fa-shopping-cart');
                        icon.removeClass('animate__animated animate__rubberBand');
                        void icon[0].offsetWidth;
                        icon.addClass('animate__animated animate__rubberBand');
                    }
                });
                
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
    // Mobile sidebar toggle with backdrop
    const toggleBtn = document.getElementById('sidebar-toggle');
    const sidebar = document.querySelector('.sidebar');
    
    if (toggleBtn && sidebar) {
        // Create backdrop element if it doesn't exist
        let backdrop = document.querySelector('.sidebar-backdrop');
        if (!backdrop) {
            backdrop = document.createElement('div');
            backdrop.className = 'sidebar-backdrop';
            document.body.appendChild(backdrop);
        }

        toggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            sidebar.classList.toggle('open');
            backdrop.classList.toggle('show');
            document.body.classList.toggle('sidebar-open');
            
            // Animate hamburger icon if present
            const icon = toggleBtn.querySelector('.hamburger-icon');
            if (icon) icon.classList.toggle('active');
        });

        backdrop.addEventListener('click', () => {
            sidebar.classList.remove('open');
            backdrop.classList.remove('show');
            document.body.classList.remove('sidebar-open');
            
            const icon = toggleBtn.querySelector('.hamburger-icon');
            if (icon) icon.classList.remove('active');
        });

        // Close sidebar on link click (mobile)
        const navLinks = sidebar.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', () => {
                if (window.innerWidth < 992) {
                    sidebar.classList.remove('open');
                    backdrop.classList.remove('show');
                    document.body.classList.remove('sidebar-open');
                    
                    const icon = toggleBtn.querySelector('.hamburger-icon');
                    if (icon) icon.classList.remove('active');
                }
            });
        });
    }

    // Navbar scroll effect
    const navbar = document.querySelector('.navbar');
    const adminHeader = document.querySelector('.admin-header');
    
    window.addEventListener('scroll', () => {
        if (window.scrollY > 20) {
            if (navbar) navbar.classList.add('scrolled');
            if (adminHeader) adminHeader.classList.add('scrolled');
        } else {
            if (navbar) navbar.classList.remove('scrolled');
            if (adminHeader) adminHeader.classList.remove('scrolled');
        }
    });

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
