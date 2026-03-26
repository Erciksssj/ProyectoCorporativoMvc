document.addEventListener('DOMContentLoaded', () => {
  document.documentElement.classList.remove('no-js');
  document.documentElement.classList.add('js-enabled');

  const jsEnabledInput = document.getElementById('js-enabled-input');
  if (jsEnabledInput) {
    jsEnabledInput.value = 'true';
  }

  const refreshButton = document.getElementById('refresh-captcha');
  const captchaQuestion = document.getElementById('captcha-question');
  if (refreshButton && captchaQuestion) {
    refreshButton.addEventListener('click', async (event) => {
      const url = refreshButton.getAttribute('data-captcha-url');
      if (!url) return;

      event.preventDefault();

      try {
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        if (!response.ok) return;
        const data = await response.json();
        if (data && data.pregunta) {
          captchaQuestion.textContent = data.pregunta;
        }
      } catch (_) { }
    });
  }

  const inputImagen = document.getElementById('imagen-usuario-input');
  const preview = document.getElementById('imagen-usuario-preview');
  const wrapper = document.getElementById('imagen-usuario-preview-wrapper');
  if (inputImagen && preview && wrapper) {
    inputImagen.addEventListener('change', (e) => {
      const file = e.target.files && e.target.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = () => {
        preview.src = reader.result;
        wrapper.classList.remove('hidden');
      };
      reader.readAsDataURL(file);
    });
  }
});
