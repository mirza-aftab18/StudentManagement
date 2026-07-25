const cnicInput = document.getElementById("cnic");
const continueButton = document.getElementById("btnContinue");
const result = document.getElementById("result");

// ==========================
// CNIC Verification
// ==========================

if (cnicInput && continueButton && result) {

    cnicInput.addEventListener("input", function () {

        let value = this.value.replace(/\D/g, "");

        if (value.length > 13)
            value = value.substring(0, 13);

        let formatted = "";

        if (value.length > 0)
            formatted += value.substring(0, 5);

        if (value.length > 5)
            formatted += "-" + value.substring(5, 12);

        if (value.length > 12)
            formatted += "-" + value.substring(12);

        this.value = formatted;

        continueButton.disabled = true;
        result.innerHTML = "";

        if (formatted.length !== 15)
            return;

        result.innerHTML =
            "<span class='text-primary'>⏳ Checking CNIC...</span>";

        fetch("/Home/CheckCNIC", {

            method: "POST",

            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },

            body: "cnic=" + encodeURIComponent(formatted)

        })

            .then(response => response.json())

            .then(data => {

                if (data.exists) {

                    result.innerHTML =
                        "<span class='text-danger'>✖ This CNIC is already registered.</span>";

                    continueButton.disabled = true;

                }
                else {

                    result.innerHTML =
                        "<span class='text-success'>✔ CNIC is available. You can continue registration.</span>";

                    continueButton.disabled = false;

                }

            })

            .catch(() => {

                result.innerHTML =
                    "<span class='text-danger'>Something went wrong.</span>";

                continueButton.disabled = true;

            });

    });

}

// ==========================
// Image Preview
// ==========================

const profileImage = document.getElementById("profileImage");
const imagePreview = document.getElementById("imagePreview");

if (profileImage && imagePreview) {

    profileImage.addEventListener("change", function () {

        const file = this.files[0];

        if (!file) {

            imagePreview.src = "#";
            imagePreview.style.display = "none";
            return;

        }

        const reader = new FileReader();

        reader.onload = function (e) {

            imagePreview.src = e.target.result;
            imagePreview.style.display = "block";

        };

        reader.readAsDataURL(file);

    });

}