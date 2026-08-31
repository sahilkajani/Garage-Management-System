import { Component, inject, OnInit, signal } from '@angular/core';

import { WeatherForecast } from '../../models/weather-forecast.model';
import { WeatherService } from '../../services/weather.service';

interface StatCard {
  label: string;
  value: string;
  detail: string;
  accent: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private readonly weatherService = inject(WeatherService);

  protected readonly stats: StatCard[] = [
    { label: 'Open Work Orders', value: '12', detail: '3 due today', accent: 'blue' },
    { label: 'Vehicles In Bay', value: '8', detail: '2 awaiting parts', accent: 'green' },
    { label: 'Customers', value: '148', detail: '6 new this week', accent: 'purple' },
    { label: 'Revenue (MTD)', value: '£24.8k', detail: '+8% vs last month', accent: 'amber' },
  ];

  protected readonly forecasts = signal<WeatherForecast[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.weatherService.getForecasts().subscribe({
      next: (forecasts) => {
        this.forecasts.set(forecasts);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to reach the API. Start the backend with dotnet run in the api folder.');
        this.loading.set(false);
      },
    });
  }
}
