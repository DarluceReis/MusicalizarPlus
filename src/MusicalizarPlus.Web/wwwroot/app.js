document.addEventListener("click", (event) => {
    const button = event.target.closest("[data-toggle-password]");
    if (!button) {
        return;
    }

    const input = document.getElementById(button.dataset.togglePassword);
    if (!input) {
        return;
    }

    const isHidden = input.type === "password";
    input.type = isHidden ? "text" : "password";
    button.classList.remove("is-closing");
    button.classList.toggle("is-visible", isHidden);
    if (!isHidden) {
        button.classList.add("is-closing");
        window.setTimeout(() => button.classList.remove("is-closing"), 520);
    }
    button.setAttribute("aria-label", isHidden ? "Esconder senha" : "Mostrar senha");
});

window.musicalizarProfile = {
    initPhoto(targetId, key) {
        const target = document.getElementById(targetId);
        if (!target) {
            return;
        }

        this.applyPhoto(target, localStorage.getItem(key));
    },
    async choosePhoto(event) {
        const input = event.target;
        const file = input.files && input.files[0];
        const target = document.getElementById(input.dataset.profilePhotoTarget);

        if (!file || !target || !file.type.startsWith("image/")) {
            return;
        }

        try {
            const dataUrl = await this.createAvatarDataUrl(file);
            localStorage.setItem(input.dataset.profilePhotoKey, dataUrl);
            this.applyPhoto(target, dataUrl);
        } catch {
            alert("Não foi possível carregar a foto. Tente outra imagem.");
        } finally {
            input.value = "";
        }
    },
    removePhoto(event) {
        const button = event.currentTarget;
        const target = document.getElementById(button.dataset.profilePhotoTarget);

        if (target) {
            localStorage.setItem(button.dataset.profilePhotoKey, "removed");
            this.applyPhoto(target, "removed");
        }
    },
    applyPhoto(target, value) {
        if (!value) {
            target.classList.remove("avatar-empty");
            target.style.backgroundImage = "";
            return;
        }

        if (value === "removed") {
            target.classList.add("avatar-empty");
            target.style.backgroundImage = "none";
            return;
        }

        target.classList.remove("avatar-empty");
        target.style.backgroundImage = `url("${value}")`;
    },
    createAvatarDataUrl(file) {
        return new Promise((resolve, reject) => {
            const image = new Image();
            const reader = new FileReader();

            reader.addEventListener("error", reject);
            reader.addEventListener("load", () => {
                image.src = reader.result;
            });

            image.addEventListener("error", reject);
            image.addEventListener("load", () => {
                const size = 320;
                const sourceSize = Math.min(image.width, image.height);
                const sourceX = Math.floor((image.width - sourceSize) / 2);
                const sourceY = Math.floor((image.height - sourceSize) / 2);
                const canvas = document.createElement("canvas");
                canvas.width = size;
                canvas.height = size;

                const context = canvas.getContext("2d");
                context.drawImage(image, sourceX, sourceY, sourceSize, sourceSize, 0, 0, size, size);
                resolve(canvas.toDataURL("image/jpeg", 0.82));
            });

            reader.readAsDataURL(file);
        });
    }
};

localStorage.removeItem("musicalizarplus.lang");

