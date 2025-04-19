// Line chart: Temperatures (morning, afternoon, evening, night)
window.renderTempChart = (temps) => {
    const canvas = document.getElementById("tempChart");
    if (!canvas) {
        console.warn("Canvas not found!");
        return;
    }

    const ctx = canvas.getContext("2d");
    new Chart(ctx, {
        type: "line",
        data: {
            labels: ["Morning", "Afternoon", "Evening", "Night"],
            datasets: [{
                label: "Temperature (°C)",
                data: temps,
                borderColor: "rgba(75, 192, 192, 1)",
                backgroundColor: "rgba(75, 192, 192, 0.2)",
                borderWidth: 2,
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: false
                }
            }
        }
    });
};

// Doughnut chart: wind, rain, pressure
window.renderPieChart = (canvasId, value, maxValue, label) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.warn(`Canvas ${canvasId} not found!`);
        return;
    }

    const ctx = canvas.getContext("2d");

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: [label, "Remaining"],
            datasets: [{
                data: [value, Math.max(0, maxValue - value)],
                backgroundColor: [
                    "rgba(54, 162, 235, 0.8)",
                    "rgba(200, 200, 200, 0.3)"
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            cutout: "75%",
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (ctx) {
                            return `${ctx.label}: ${ctx.raw}`;
                        }
                    }
                }
            }
        }
    });
};