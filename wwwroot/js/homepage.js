/*
FOOTBALLER WEB APP - HOMEPAGE JAVASCRIPT
Funciones para carousel, animaciones y scroll suave
Última actualización: 2025-10-05
*/

// Función de scroll suave hacia elementos específicos
window.scrollToElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start',
            inline: 'nearest'
        });
    }
};

// Función para detectar dispositivos móviles
window.isMobileDevice = () => {
    return window.innerWidth <= 768 || /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
};

// Inicialización principal cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Smooth scrolling para enlaces con ancla
    const links = document.querySelectorAll('a[href^="#"]');
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href').substring(1);
            window.scrollToElement(targetId);
        });
    });

    // Configuración de animaciones con Intersection Observer
    const observerOptions = {
        threshold: 0.15,
        rootMargin: '0px 0px -80px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                // Animación escalonada para tarjetas de características
                setTimeout(() => {
                    entry.target.classList.add('animate-in');
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, index * 150); // Retraso de 150ms entre cada elemento
            }
        });
    }, observerOptions);

    // Observar elementos para animaciones
    const animatedElements = document.querySelectorAll('.feature-card');
    animatedElements.forEach((el, index) => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(50px)';
        el.style.transition = 'opacity 0.8s ease, transform 0.8s ease';
        observer.observe(el);
    });

    // Efectos de parallax solo en desktop para mejor rendimiento
    if (!window.isMobileDevice()) {
        let ticking = false;
        
        const updateParallax = () => {
            const scrolled = window.pageYOffset;
            const activeSlide = document.querySelector('.carousel-slide.active');
            
            if (activeSlide) {
                const speed = 0.3;
                activeSlide.style.transform = `translateY(${scrolled * speed}px)`;
            }
            
            ticking = false;
        };

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        });
    }

    // Inicializar animaciones adicionales
    initializeAdditionalAnimations();
});

// Precargar imágenes del carousel para mejor rendimiento
window.preloadCarouselImages = (imageUrls) => {
    if (Array.isArray(imageUrls)) {
        imageUrls.forEach(url => {
            const img = new Image();
            img.onload = () => console.log(`Imagen precargada: ${url}`);
            img.onerror = () => console.warn(`Error cargando imagen: ${url}`);
            img.src = url;
        });
    }
};

// Inicializar animaciones adicionales
const initializeAdditionalAnimations = () => {
    // Agregar estilos CSS dinámicos para animaciones
    const style = document.createElement('style');
    style.textContent = `
        .animate-in {
            animation: slideInUp 0.8s ease-out forwards;
        }
        
        @keyframes slideInUp {
            from {
                opacity: 0;
                transform: translateY(50px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .feature-card:hover .feature-icon {
            animation: bounceIcon 0.6s ease-in-out;
        }
        
        @keyframes bounceIcon {
            0%, 100% { transform: scale(1) rotate(0deg); }
            25% { transform: scale(1.1) rotate(5deg); }
            75% { transform: scale(1.05) rotate(-2deg); }
        }
        
        /* Animación de carga para el spinner */
        .loading-container .spinner-border {
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    `;
    document.head.appendChild(style);
    
    // Marcar que las animaciones están habilitadas
    document.body.classList.add('animations-enabled');
};

// Lazy loading para imágenes (si se implementa en el futuro)
window.initializeLazyLoading = () => {
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                img.classList.add('loaded');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => imageObserver.observe(img));
};

// Manejar cambios de tamaño de ventana
window.addEventListener('resize', () => {
    // Recalcular elementos si es necesario
    const carousel = document.querySelector('.hero-carousel-section');
    if (carousel && window.isMobileDevice()) {
        carousel.style.minHeight = '450px';
    }
});