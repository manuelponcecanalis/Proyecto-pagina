// Calcular altura dinámica del header y establecer variable CSS
(function () {
    const header = document.querySelector('.header');
    if (!header) return;

    const setHeaderHeight = () => {
        const h = Math.ceil(header.getBoundingClientRect().height);
        document.documentElement.style.setProperty('--header-h', `${h}px`);
    };

    // Establecer altura inicial
    setHeaderHeight();

    // Actualizar si cambia por responsive / fonts / menú
    window.addEventListener('resize', setHeaderHeight);
    if (window.ResizeObserver) {
        new ResizeObserver(setHeaderHeight).observe(header);
    }
})();

// Menú hamburguesa para móviles
const menuToggle = document.querySelector('.menu-toggle');
const nav = document.querySelector('.nav');

if (menuToggle && nav) {
    menuToggle.addEventListener('click', () => {
        menuToggle.classList.toggle('active');
        nav.classList.toggle('active');
        if (nav.classList.contains('active')) {
            document.body.classList.add('menu-open');
        } else {
            document.body.classList.remove('menu-open');
        }
    });

    // Manejar desplegables en móviles (clic)
    document.querySelectorAll('.nav-link-dropdown').forEach(dropdownLink => {
        dropdownLink.addEventListener('click', (e) => {
            if (window.innerWidth <= 768) {
                e.preventDefault();
                const dropdown = dropdownLink.closest('.nav-dropdown');
                dropdown.classList.toggle('active');
            }
        });
    });

    // Cerrar menú al hacer clic en un enlace (excepto dropdowns)
    document.querySelectorAll('.nav-link:not(.nav-link-dropdown), .btn-login, .dropdown-item, .dropdown-card').forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 768) {
                menuToggle.classList.remove('active');
                nav.classList.remove('active');
                document.body.classList.remove('menu-open');
                // Cerrar todos los desplegables
                document.querySelectorAll('.nav-dropdown').forEach(dropdown => {
                    dropdown.classList.remove('active');
                });
            }
        });
    });

    // Cerrar menú al hacer clic fuera o al hacer scroll
    document.addEventListener('click', (e) => {
        if (!nav.contains(e.target) && !menuToggle.contains(e.target) && nav.classList.contains('active')) {
            menuToggle.classList.remove('active');
            nav.classList.remove('active');
            document.body.style.overflow = '';
        }
    });

    // Cerrar menú al hacer scroll en móviles
    let lastScrollTop = 0;
    window.addEventListener('scroll', () => {
        if (window.innerWidth <= 768 && nav.classList.contains('active')) {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            if (Math.abs(scrollTop - lastScrollTop) > 10) {
                menuToggle.classList.remove('active');
                nav.classList.remove('active');
                document.body.style.overflow = '';
            }
            lastScrollTop = scrollTop;
        }
    });

    // Cerrar menú al redimensionar la ventana
    window.addEventListener('resize', () => {
        if (window.innerWidth > 768 && nav.classList.contains('active')) {
            menuToggle.classList.remove('active');
            nav.classList.remove('active');
            document.body.style.overflow = '';
        }
    });
}

// Smooth scroll para los enlaces de navegación
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        const href = this.getAttribute('href');
        if (href === '#' || href === '#login') return; // Ignorar anchors vacíos o login
        
        const target = document.querySelector(href);
        
        // Si el target es un tool-card, prevenir el scroll y no hacer nada (cards estáticas)
        if (target && target.classList.contains('tool-card')) {
            e.preventDefault();
            return; // Las cards permanecen estáticas, no se hace scroll
        }
        
        // Para otros elementos, permitir el scroll normal
        if (target && !target.classList.contains('tool-card')) {
            e.preventDefault();
            const header = document.querySelector('.header');
            const headerHeight = header ? Math.ceil(header.getBoundingClientRect().height) : 72;
            const elementPosition = target.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - headerHeight - 12;
            
            window.scrollTo({
                top: Math.max(0, offsetPosition),
                behavior: 'smooth'
            });
        }
    });
});

// Corregir scroll cuando se carga la página con un hash en la URL
window.addEventListener('load', () => {
    if (window.location.hash) {
        setTimeout(() => {
            const target = document.querySelector(window.location.hash);
            if (target) {
                // Si es un tool-card, no hacer scroll (cards estáticas)
                if (target.classList.contains('tool-card')) {
                    // Limpiar el hash de la URL sin hacer scroll
                    history.replaceState(null, null, window.location.pathname + window.location.search);
                    return;
                }
                
                // Para otros elementos, usar scroll normal con compensación
                const header = document.querySelector('.header');
                const headerHeight = header ? Math.ceil(header.getBoundingClientRect().height) : 72;
                const elementPosition = target.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.pageYOffset - headerHeight - 12;
                
                window.scrollTo({
                    top: Math.max(0, offsetPosition),
                    behavior: 'smooth'
                });
            }
        }, 100);
    }
});

// Efecto de scroll en el header (agregar sombra cuando se hace scroll)
let lastScroll = 0;
const header = document.querySelector('.header');

window.addEventListener('scroll', () => {
    const currentScroll = window.pageYOffset;
    
    if (currentScroll > 50) {
        header.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)';
    } else {
        header.style.boxShadow = '0 2px 4px rgba(0, 0, 0, 0.1)';
    }
    
    lastScroll = currentScroll;
});

// Funcionalidad del buscador
const searchInput = document.querySelector('.search-input');
const searchBtn = document.querySelector('.search-btn');

searchBtn.addEventListener('click', () => {
    const query = searchInput.value.trim();
    if (query) {
        // Aquí puedes agregar la lógica de búsqueda
        console.log('Buscando:', query);
        // Ejemplo: window.location.href = `/buscar?q=${encodeURIComponent(query)}`;
    }
});

searchInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
        searchBtn.click();
    }
});

// Animación de entrada para las tarjetas (Intersection Observer)
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, observerOptions);

// Aplicar animación a las tarjetas
document.querySelectorAll('.tool-card').forEach((card, index) => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(20px)';
    card.style.transition = `opacity 0.6s ease ${index * 0.1}s, transform 0.6s ease ${index * 0.1}s`;
    observer.observe(card);
});

// Efecto parallax eliminado - el hero debe permanecer en flujo normal sin transform

// Agregar efecto de hover mejorado a las tarjetas
document.querySelectorAll('.tool-card').forEach(card => {
    card.addEventListener('mouseenter', function() {
        this.style.transition = 'all 0.3s ease';
    });
});

// Funcionalidad del botón CTA
const ctaButton = document.querySelector('.cta-button');
if (ctaButton) {
    ctaButton.addEventListener('click', () => {
        // Aquí puedes agregar la lógica para subir apuntes
        console.log('Redirigir a subir apunte');
        // Ejemplo: window.location.href = '/subir-apunte';
    });
}

// Mejorar accesibilidad: focus visible en elementos interactivos
document.querySelectorAll('a, button').forEach(element => {
    element.addEventListener('focus', function() {
        this.style.outline = `2px solid ${getComputedStyle(document.documentElement).getPropertyValue('--color-accent')}`;
        this.style.outlineOffset = '2px';
    });
    
    element.addEventListener('blur', function() {
        this.style.outline = 'none';
    });
});

// Cargar publicaciones de Instagram
// Usando el método de embed de Instagram o SnapWidget
function loadInstagramPosts() {
    const feedContainer = document.getElementById('instagram-embed-container');
    if (!feedContainer) return;

    // Opción recomendada: Usar SnapWidget (gratuito, fácil de configurar)
    // 1. Visita https://snapwidget.com
    // 2. Crea una cuenta gratuita
    // 3. Crea un nuevo widget con tu usuario: proyectoeconomicas
    // 4. Copia el código del iframe y reemplaza el src del iframe abajo
    
    feedContainer.innerHTML = `
        <iframe src="https://snapwidget.com/embed/code/proyectoeconomicas" 
                class="snapwidget-widget" 
                allowtransparency="true" 
                frameborder="0" 
                scrolling="no" 
                style="border:none; overflow:hidden; width:100%; height:100%;">
        </iframe>
    `;

    // Alternativa: Si tienes acceso a Instagram Graph API, descomenta esto:

    // Opción 1: Usar el widget embebido de Instagram (recomendado)
    // Descomenta esto y reemplaza 'TU_USERNAME' con tu usuario de Instagram
    /*
    const script = document.createElement('script');
    script.src = 'https://www.instagram.com/embed.js';
    script.async = true;
    document.body.appendChild(script);
    
    // O usar el iframe embed
    feedContainer.innerHTML = `
        <blockquote class="instagram-media" data-instgrm-permalink="https://www.instagram.com/TU_USERNAME/" data-instgrm-version="14" style="background:#FFF; border:0; border-radius:3px; box-shadow:0 0 1px 0 rgba(0,0,0,0.5),0 1px 10px 0 rgba(0,0,0,0.15); margin: 1px; max-width:540px; min-width:326px; padding:0; width:99.375%; width:-webkit-calc(100% - 2px); width:calc(100% - 2px);">
        <div style="padding:16px;">
            <a href="https://www.instagram.com/TU_USERNAME/" style="background:#FFFFFF; line-height:0; padding:0 0; text-align:center; text-decoration:none; width:100%;" target="_blank">
                <div style="display: flex; flex-direction: row; align-items: center;">
                    <div style="background-color: #F4F4F4; border-radius: 50%; flex-grow: 0; height: 40px; margin-right: 14px; width: 40px;"></div>
                    <div style="display: flex; flex-direction: column; flex-grow: 1; justify-content: center;">
                        <div style="background-color: #F4F4F4; border-radius: 4px; flex-grow: 0; height: 14px; margin-bottom: 6px; width: 100px;"></div>
                        <div style="background-color: #F4F4F4; border-radius: 4px; flex-grow: 0; height: 14px; width: 60px;"></div>
                    </div>
                </div>
            </a>
        </div>
        <p style="color:#c9c8cd; font-family:Arial,sans-serif; font-size:14px; line-height:17px; margin-bottom:0; margin-top:8px; overflow:hidden; padding:8px 0 7px; text-align:center; text-overflow:ellipsis; white-space:nowrap;">
            <a href="https://www.instagram.com/TU_USERNAME/" style="color:#c9c8cd; font-family:Arial,sans-serif; font-size:14px; font-style:normal; font-weight:normal; line-height:17px; text-decoration:none;" target="_blank">Ver esta publicación en Instagram</a>
        </p>
    </blockquote>
    `;
    */

    // Opción 2: Cargar publicaciones usando la API de Instagram Graph (requiere token de acceso)
    // Reemplaza con tu token de acceso y user ID
    /*
    const ACCESS_TOKEN = 'TU_ACCESS_TOKEN';
    const USER_ID = 'TU_USER_ID';
    
    fetch(`https://graph.instagram.com/${USER_ID}/media?fields=id,media_type,media_url,permalink,caption&access_token=${ACCESS_TOKEN}&limit=6`)
        .then(response => response.json())
        .then(data => {
            if (data.data) {
                feedContainer.innerHTML = '';
                data.data.forEach(post => {
                    const postElement = createInstagramPost(post);
                    feedContainer.appendChild(postElement);
                });
            }
        })
        .catch(error => {
            console.error('Error cargando publicaciones de Instagram:', error);
            feedContainer.innerHTML = '<div class="instagram-placeholder"><p>No se pudieron cargar las publicaciones. Visita nuestro <a href="https://instagram.com/proyectoeconomicas" target="_blank">Instagram</a>.</p></div>';
        });
    */

    // Opción 3: Publicaciones de ejemplo (solo si SnapWidget no funciona)
    // Descomenta esto solo como fallback
    /*
    const examplePosts = [
        { id: 1, image: 'https://via.placeholder.com/400/003366/FFFFFF?text=Post+1', likes: 120, comments: 15, permalink: '#' },
        { id: 2, image: 'https://via.placeholder.com/400/E91E63/FFFFFF?text=Post+2', likes: 89, comments: 8, permalink: '#' },
        { id: 3, image: 'https://via.placeholder.com/400/003366/FFFFFF?text=Post+3', likes: 156, comments: 22, permalink: '#' },
        { id: 4, image: 'https://via.placeholder.com/400/E91E63/FFFFFF?text=Post+4', likes: 203, comments: 31, permalink: '#' },
        { id: 5, image: 'https://via.placeholder.com/400/003366/FFFFFF?text=Post+5', likes: 98, comments: 12, permalink: '#' },
        { id: 6, image: 'https://via.placeholder.com/400/E91E63/FFFFFF?text=Post+6', likes: 167, comments: 19, permalink: '#' }
    ];

    feedContainer.innerHTML = '';
    examplePosts.forEach(post => {
        const postElement = createInstagramPostElement(post);
        feedContainer.appendChild(postElement);
    });
    */
}

function createInstagramPostElement(post) {
    const postDiv = document.createElement('div');
    postDiv.className = 'instagram-post';
    postDiv.onclick = () => {
        if (post.permalink && post.permalink !== '#') {
            window.open(post.permalink, '_blank', 'noopener,noreferrer');
        }
    };

    postDiv.innerHTML = `
        <img src="${post.image}" alt="Publicación de Instagram" loading="lazy">
        <div class="instagram-post-overlay">
            <div class="instagram-post-info">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                </svg>
                <span>${post.likes || 0}</span>
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-left: 1rem;">
                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
                </svg>
                <span>${post.comments || 0}</span>
            </div>
        </div>
    `;

    return postDiv;
}

// Toggle del formulario de contacto
function toggleContactForm() {
    const formContainer = document.getElementById('contact-form-container');
    if (formContainer) {
        formContainer.classList.toggle('expanded');
        
        // Si se está expandiendo, hacer scroll suave al formulario
        if (formContainer.classList.contains('expanded')) {
            setTimeout(() => {
                formContainer.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }, 100);
        }
    }
}

// Toggle del formulario de sumarse (página Nosotros)
function toggleJoinForm() {
    const joinBox = document.getElementById('join-box');
    if (!joinBox) return;

    joinBox.classList.toggle('expanded');

    if (joinBox.classList.contains('expanded')) {
        setTimeout(() => {
            joinBox.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }, 100);
    }
}

// Manejar envío del formulario de contacto
const contactForm = document.getElementById('contact-form');
if (contactForm) {
    contactForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        // Aquí puedes agregar la lógica para enviar el formulario
        const formData = {
            nombre: document.getElementById('nombre').value,
            mail: document.getElementById('mail').value,
            asunto: document.getElementById('asunto').value,
            mensaje: document.getElementById('mensaje').value
        };
        
        console.log('Formulario enviado:', formData);
        
        // Mostrar notificación
        showNotification();
        
        // Limpiar formulario y cerrarlo después de un breve delay
        setTimeout(() => {
            contactForm.reset();
            toggleContactForm();
        }, 500);
    });
}

// Función para mostrar la notificación
function showNotification() {
    const notification = document.getElementById('contact-notification');
    if (notification) {
        notification.classList.add('show');
        
        // Ocultar la notificación después de 4 segundos
        setTimeout(() => {
            notification.classList.remove('show');
        }, 4000);
    }
}

// Carrusel de imágenes en el hero
// Ticker Bar - Items del ticker (fácil de editar)
const TICKER_ITEMS = [
    'Producción industrial −12,4%',
    'Construcción −18,7%',
    'Obra pública: 0 nuevas licitaciones',
    'Empleo industrial −94.000 puestos',
    'Salario real −21%',
    'Presupuesto universitario −30%',
    'Ciencia y técnica: proyectos paralizados',
    'Incendios forestales: +1,2M ha afectadas'
];

// Inicializar Ticker Bar
function initTickerBar() {
    const tickerBar = document.querySelector('.ticker-bar');
    if (!tickerBar) return;

    // Verificar si el usuario prefiere movimiento reducido
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    
    if (prefersReducedMotion) {
        // Si prefiere movimiento reducido, no hacer nada - el CSS ya maneja el scroll horizontal
        return;
    }

    // Pausar animación al hacer hover (ya está en CSS, pero por si acaso)
    tickerBar.addEventListener('mouseenter', () => {
        const wrapper = tickerBar.querySelector('.ticker-bar-wrapper');
        if (wrapper) {
            wrapper.style.animationPlayState = 'paused';
        }
    });

    tickerBar.addEventListener('mouseleave', () => {
        const wrapper = tickerBar.querySelector('.ticker-bar-wrapper');
        if (wrapper) {
            wrapper.style.animationPlayState = 'running';
        }
    });
}

function initHeroCarousel() {
    const slides = document.querySelectorAll('.hero-slide');
    const dots = document.querySelectorAll('.carousel-dots .dot');
    const prevBtn = document.querySelector('.carousel-prev');
    const nextBtn = document.querySelector('.carousel-next');
    
    if (slides.length === 0) return;

    let currentSlide = 0;
    let carouselInterval;

    function showSlide(index) {
        // Remover active de todos los slides y dots
        slides.forEach(slide => slide.classList.remove('active'));
        dots.forEach(dot => dot.classList.remove('active'));
        
        // Agregar active al slide y dot actual
        slides[index].classList.add('active');
        if (dots[index]) dots[index].classList.add('active');
        
        currentSlide = index;
    }

    function nextSlide() {
        const nextIndex = (currentSlide + 1) % slides.length;
        showSlide(nextIndex);
    }

    function prevSlide() {
        const prevIndex = (currentSlide - 1 + slides.length) % slides.length;
        showSlide(prevIndex);
    }

    function startCarousel() {
        carouselInterval = setInterval(nextSlide, 5000);
    }

    function stopCarousel() {
        clearInterval(carouselInterval);
    }

    // Event listeners para botones
    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            stopCarousel();
            nextSlide();
            startCarousel();
        });
    }

    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            stopCarousel();
            prevSlide();
            startCarousel();
        });
    }

    // Event listeners para dots
    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            stopCarousel();
            showSlide(index);
            startCarousel();
        });
    });

    // Pausar carrusel al hacer hover
    const carousel = document.querySelector('.hero-carousel');
    if (carousel) {
        carousel.addEventListener('mouseenter', stopCarousel);
        carousel.addEventListener('mouseleave', startCarousel);
    }

    // Iniciar carrusel automático
    startCarousel();
}

document.addEventListener('DOMContentLoaded', () => {
    initHeroCarousel();
    initTickerBar();
    loadInstagramPosts();
    
    // Animar el contenedor de Instagram cuando aparece en el viewport
    const instagramObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    // Observar el contenedor de Instagram después de cargar
    setTimeout(() => {
        const instagramContainer = document.querySelector('.instagram-embed-container');
        if (instagramContainer) {
            instagramContainer.style.opacity = '0';
            instagramContainer.style.transform = 'translateY(20px)';
            instagramContainer.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            instagramObserver.observe(instagramContainer);
        }
    }, 100);
});

