
const api = "/api";

async function request(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...options.headers
        }
    });

    const text = await response.text();

    if (!response.ok) {
        throw new Error(text || "Request failed.");
    }

    if (!text) return null;

    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}

function showSection(id) {
    document.querySelectorAll("main section").forEach(section => {
        section.hidden = section.id !== id;
    });

    if (id === "courses") loadCourses();
    if (id === "learners") loadLearners();
    if (id === "enrollments") loadEnrollments();
}

async function loadCourses() {
    try {
        const courses = await request(`${api}/course`);
        displayCourses(courses);
    } catch (error) {
        document.getElementById("courseList").textContent = error.message;
    }
}

async function loadAffordable() {
    try {
        const courses = await request(`${api}/course/affordable`);
        displayCourses(courses);
    } catch (error) {
        document.getElementById("courseList").textContent = error.message;
    }
}

async function loadSorted() {
    try {
        const courses = await request(`${api}/course/sort`);
        displayCourses(courses);
    } catch (error) {
        document.getElementById("courseList").textContent = error.message;
    }
}

function displayCourses(courses) {
    const list = document.getElementById("courseList");
    list.replaceChildren();

    if (!courses.length) {
        list.textContent = "No courses found.";
        return;
    }

    courses.forEach(course => {
        const card = document.createElement("article");
        card.className = "course-card";

        const title = document.createElement("h3");
        title.textContent = course.title;

        const details = [
            `Code: ${course.courseCode}`,
            `Fee: ₹${course.fee}`,
            `Duration: ${course.durationDays} days`,
            `Available seats: ${course.availableSeats}`
        ];

        card.appendChild(title);

        details.forEach(detail => {
            const p = document.createElement("p");
            p.textContent = detail;
            card.appendChild(p);
        });

        list.appendChild(card);
    });
}

document.getElementById("learnerForm")
    .addEventListener("submit", async event => {
        event.preventDefault();

        const learner = {
            learnerId: document.getElementById("learnerId").value.trim(),
            name: document.getElementById("learnerName").value.trim(),
            contactNumber: document.getElementById("contactNumber").value.trim(),
            email: document.getElementById("email").value.trim()
        };

        try {
            const result = await request(`${api}/learner`, {
                method: "POST",
                body: JSON.stringify(learner)
            });

            document.getElementById("learnerMessage").textContent = result;
            event.target.reset();
            loadLearners();
        } catch (error) {
            document.getElementById("learnerMessage").textContent = error.message;
        }
    });

async function loadLearners() {
    try {
        const learners = await request(`${api}/learner`);
        const list = document.getElementById("learnerList");
        list.replaceChildren();

        if (!learners.length) {
            list.textContent = "No learners registered yet.";
            return;
        }

        const table = document.createElement("table");
        table.innerHTML = `
            <thead>
                <tr><th>ID</th><th>Name</th><th>Contact</th><th>Email</th></tr>
            </thead>
        `;

        const tbody = document.createElement("tbody");

        learners.forEach(learner => {
            const row = document.createElement("tr");

            [learner.learnerId, learner.name,
            learner.contactNumber, learner.email].forEach(value => {
                const cell = document.createElement("td");
                cell.textContent = value;
                row.appendChild(cell);
            });

            tbody.appendChild(row);
        });

        table.appendChild(tbody);

        const wrapper = document.createElement("div");
        wrapper.className = "table-wrap";
        wrapper.appendChild(table);
        list.appendChild(wrapper);
    } catch (error) {
        document.getElementById("learnerList").textContent = error.message;
    }
}

document.getElementById("enrollmentForm")
    .addEventListener("submit", async event => {
        event.preventDefault();

        const enrollment = {
            learnerId: document.getElementById("enrollLearnerId").value.trim(),
            courseCode: document.getElementById("enrollCourseCode").value.trim()
        };

        try {
            const result = await request(`${api}/enrollment`, {
                method: "POST",
                body: JSON.stringify(enrollment)
            });

            document.getElementById("enrollmentMessage").textContent = result;
            event.target.reset();
            loadEnrollments();
            loadCourses();
        } catch (error) {
            document.getElementById("enrollmentMessage").textContent = error.message;
        }
    });

async function loadEnrollments() {
    try {
        const enrollments = await request(`${api}/enrollment`);
        const list = document.getElementById("enrollmentList");
        list.replaceChildren();

        if (!enrollments.length) {
            list.textContent = "No enrollments yet.";
            return;
        }

        const table = document.createElement("table");
        table.innerHTML = `
            <thead>
                <tr>
                    <th>ID</th><th>Learner</th><th>Course</th>
                    <th>Final Fee</th><th>Status</th>
                </tr>
            </thead>
        `;

        const tbody = document.createElement("tbody");

        enrollments.forEach(enrollment => {
            const row = document.createElement("tr");

            [
                enrollment.enrollmentId,
                enrollment.learnerId,
                enrollment.courseCode,
                `₹${enrollment.finalFee}`,
                enrollment.status
            ].forEach(value => {
                const cell = document.createElement("td");
                cell.textContent = value;
                row.appendChild(cell);
            });

            tbody.appendChild(row);
        });

        table.appendChild(tbody);

        const wrapper = document.createElement("div");
        wrapper.className = "table-wrap";
        wrapper.appendChild(table);
        list.appendChild(wrapper);
    } catch (error) {
        document.getElementById("enrollmentList").textContent = error.message;
    }
}

loadCourses();
