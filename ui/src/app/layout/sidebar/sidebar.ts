import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavItem {
  label: string;
  route: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  protected readonly navItems: NavItem[] = [
    { label: 'Dashboard', route: '/', icon: 'dashboard' },
    { label: 'Vehicles', route: '/vehicles', icon: 'vehicles' },
    { label: 'Work Orders', route: '/work-orders', icon: 'work-orders' },
    { label: 'Customers', route: '/customers', icon: 'customers' },
  ];
}
